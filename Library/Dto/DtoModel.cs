using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace PatientEvaluationApp_UTCN.Lib
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UpdateValueAttribute : Attribute
    {
    }
    public interface IDtoModel : INotifyPropertyChanged
    {
        #region Properties
        [JsonIgnore]
        Boolean IsValid { get; }

        [JsonIgnore]
        Boolean HasChanges { get; }
        #endregion

        #region Public Methods
        void AcceptChanges();
        #endregion
    }

    public abstract class DtoModel : IDtoModel
    {
        #region Delegates/Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        protected DtoModel()
        {
            // init all IDtoProperty properties with the default value
            var dataPropertyInfos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            dataPropertyInfos = dataPropertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();

            foreach (var dataPropertyInfo in dataPropertyInfos)
            {
                var argument = DtoModel.GetDefault(dataPropertyInfo.PropertyType.GetGenericTypeDefinition());
                var value = (IDtoProperty)Activator.CreateInstance(dataPropertyInfo.PropertyType, argument);
                if (dataPropertyInfo.SetMethod != null) dataPropertyInfo.SetValue(this, value);

                // inject the validation attributes
                var attributes = dataPropertyInfo.GetCustomAttributes<ValidationAttribute>();
                value.AddValidationAttributes(attributes, this);
            }
        }
        #endregion

        #region Properties
        [JsonIgnore]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public virtual Boolean IsValid
        {
            get
            {
                var result = true;
                var propertyInfos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                // validate all regular properties who are not type of IDtoProperty
                var noDtoPropertyInfos = propertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.All(i => i != typeof(IDtoProperty))).ToArray();
                foreach (var noDtoPropertyInfo in noDtoPropertyInfos)
                {
                    var attributes = noDtoPropertyInfo.GetCustomAttributes<ValidationAttribute>().ToArray();
                    foreach (var attribute in attributes)
                    {
                        var propertyValue = noDtoPropertyInfo.GetValue(this);
                        if (!attribute.IsValid(propertyValue)) result = false;
                    }
                }

                // validate all IDtoProperty properties
                var dtoPropertyInfos = propertyInfos.Where(w => typeof(IEnumerable<IDtoModel>).IsAssignableFrom(w.PropertyType) || ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();
                foreach (var dtoPropertyInfo in dtoPropertyInfos)
                {
                    var dataValue = dtoPropertyInfo.GetValue(this);
                    if (dataValue is IEnumerable enumerable)
                    {
                        if (enumerable.OfType<IDtoModel>().Any(item => !item.IsValid)) result = false;
                    }
                    else
                    {
                        ((IDtoProperty)dataValue).SuspendNotifications();
                        ((IDtoProperty)dataValue).RaisePropertyChanged("Value");
                        ((IDtoProperty)dataValue).ResumeNotifications();
                        if (!((IDtoProperty)dataValue).IsValid(this)) result = false;
                    }
                }

                return result;
            }
        }

        [JsonIgnore]
        public Boolean HasChanges
        {
            get
            {
                var propertyInfos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

                // check all IDtoProperty properties
                propertyInfos = propertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = (IDtoProperty)propertyInfo.GetValue(this);
                    if (propertyValue.HasChanges) return true;
                }

                return false;
            }
        }
        #endregion

        #region Public Methods
        public void AcceptChanges()
        {
            var propertyInfos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            propertyInfos = propertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyValue = (IDtoProperty)propertyInfo.GetValue(this);
                propertyValue.AcceptChanges();
            }
        }

        public static Object GetDefault(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        public void RaisePropertyChanged([CallerMemberName] String propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void UpdateValues<T>(T item)
        {
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
            propertyInfos = propertyInfos.Where(w => w.GetCustomAttribute<UpdateValueAttribute>() != null).ToArray();

            foreach (var propertyInfo in propertyInfos)
            {
                if (((TypeInfo)propertyInfo.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty)))
                {
                    var dtoPropertySource = propertyInfo.GetValue(item);
                    var dtoPropertyInfo = dtoPropertySource?.GetType().GetProperty("Value");
                    var value = dtoPropertyInfo?.GetValue(dtoPropertySource);

                    var dtoPropertyTarget = propertyInfo.GetValue(this);
                    dtoPropertyInfo?.SetValue(dtoPropertyTarget, value);
                }
                else
                {
                    var value = propertyInfo.GetValue(item);
                    propertyInfo.SetValue(this, value);

                    this.RaisePropertyChanged(propertyInfo.Name);
                }
            }
        }
        #endregion
    }
}
