using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PatientEvaluationApp_UTCN.Lib
{
    public interface IDataModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Properties
        Boolean IsValid { get; }
        #endregion
        void InitProperties();

        void RaisePropertyChanged([CallerMemberName] String propertyName = null);

        void RegisterValueChanges();
        
    }

    public abstract class DataModel : IDataModel
    {
        #region Delegates/Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constructors
        protected DataModel(IAppSettings appSettings)
        {
            // init all IDtoProperty properties with the default value
            var dataPropertyInfos = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            dataPropertyInfos = dataPropertyInfos.Where(w => ((TypeInfo)w.FieldType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();
            foreach (var dataPropertyInfo in dataPropertyInfos)
            {
                var argument = DtoModel.GetDefault(dataPropertyInfo.FieldType.GetGenericTypeDefinition());
                var propertyValue = (IDtoProperty)Activator.CreateInstance(dataPropertyInfo.FieldType, argument);
                dataPropertyInfo.SetValue(this, propertyValue);

                // inject the validation attributes
                if (propertyValue.ValidationAttributes.Count == 0)
                {
                    var attributes = dataPropertyInfo.GetCustomAttributes<ValidationAttribute>();
                    propertyValue.AddValidationAttributes(attributes, this);
                }

                // attach to the property changed event and raise the IsValide change
                propertyValue.PropertyChanged += (s, e) => this.RaisePropertyChanged(nameof(this.IsValid));
            }
            this.AppSettings = appSettings;
        }
        #endregion

        #region Properties
        public virtual Boolean IsValid => true;


        public String Error { get; private set; }

        public String this[String columnName] => "";

        protected IAppSettings AppSettings { get; }
        #endregion

        #region Public Methods
        #region Public Methods
        public void InitProperties()
        {
            // attach to the property changed event for all properies in this data model
            // this is needed to raise the changed event for IsValid so to validate the whole model
            // also the validation attributes need to injected into each DtoProperty to be able to update the UI with the red border around the control
            // this can't be done by instanciating because serialization is in use for the API

            // get all properties for this DataModel which are type of DtoModel
            var modelPropertyInfos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            modelPropertyInfos = modelPropertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoModel))).ToArray();

            foreach (var modelPropertyInfo in modelPropertyInfos)
            {
                var modelValue = modelPropertyInfo.GetValue(this);
                if (modelValue == null) continue;

                // get all properties in this DtoModel which inmpletens the IDtoProperty interface
                this.EnumerateDtoProperties(modelPropertyInfo.PropertyType, modelValue);
            }

            // get all properties in this DtoModel which inmpletens the IDtoProperty interface
            var dataPropertyInfos = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            dataPropertyInfos = dataPropertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();

            foreach (var dataPropertyInfo in dataPropertyInfos)
            {
                // inject the validation attributes and attach to the property changed event and raise the IsValide change
                this.InjectValidationAttributes(dataPropertyInfo, this);
            }
        }

        public void RaisePropertyChanged([CallerMemberName] String propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void RegisterValueChanges()
        {
        }

        protected void RegisterValueChanges<TModel, TProperty>(TModel model, Expression<Func<TModel, TProperty>> expression) where TProperty : IDtoProperty
        {
            var targetObject = (Object)model;
            var members = new List<MemberInfo>();
            var exp = expression.Body;

            while (exp != null)
                if (exp is MemberExpression mi)
                {
                    members.Add(mi.Member);
                    exp = mi.Expression;
                }
                else
                {
                    break;
                }

            targetObject = members.OfType<PropertyInfo>().Reverse().Aggregate(targetObject, (current, pi) => pi.GetValue(current));

            if (targetObject is IDtoProperty dtoProperty)
            {
                dtoProperty.ValueChanged += (s, e) => this.OnValueChanged(members.First().Name);
            }
        }

        protected virtual void OnValueChanged(String propertyName)
        {
        }
        #endregion

        #region Private Methods
        private void EnumerateDtoProperties(Type propertyType, Object modelValue)
        {
            var dataPropertyInfos = propertyType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            // get all properties in this DtoModel which inmpletens the IDtoProperty interface
            var dtoPropertyInfos = dataPropertyInfos.Where(w => ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoProperty))).ToArray();
            foreach (var dtoPropertyInfo in dtoPropertyInfos)
            {
                // inject the validation attributes and attach to the property changed event and raise the IsValide change
                this.InjectValidationAttributes(dtoPropertyInfo, modelValue);
            }

            // get all properties for this DataModel which are type of DtoModel
            var dtoModelInfos = dataPropertyInfos.Where(w => typeof(IEnumerable<IDtoModel>).IsAssignableFrom(w.PropertyType) || ((TypeInfo)w.PropertyType).ImplementedInterfaces.Any(i => i == typeof(IDtoModel))).ToArray();
            foreach (var dtoModelInfo in dtoModelInfos)
            {
                var dataValue = dtoModelInfo.GetValue(modelValue);
                if (dataValue == null) continue;

                if (dataValue is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        this.EnumerateDtoProperties(item.GetType(), item);
                    }
                }
                else
                {
                    // get all properties in this DtoModel which inmpletens the IDtoProperty interface
                    this.EnumerateDtoProperties(dtoModelInfo.PropertyType, dataValue);
                }
            }
        }

        private void InjectValidationAttributes(PropertyInfo dataPropertyInfo, Object modelValue)
        {
            var propertyValue = (IDtoProperty)dataPropertyInfo.GetValue(modelValue);

            // inject the validation attributes
            if (propertyValue.ValidationAttributes.Count == 0)
            {
                var attributes = dataPropertyInfo.GetCustomAttributes<ValidationAttribute>();
                propertyValue.AddValidationAttributes(attributes, modelValue);
            }

            // attach to the property changed event and raise the IsValide change
            propertyValue.PropertyChanged += (s, e) =>
            {
                if (!propertyValue.IsNotificationsSuspended) this.RaisePropertyChanged(nameof(this.IsValid));
            };

            // check, if value is type of DtoModel. if so, enumerate the properties
            var valueType = propertyValue.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
            if (valueType?.PropertyType.BaseType != typeof(DtoModel)) return;
            var subValue = valueType.GetValue(propertyValue);
            if (subValue != null)
            {
                this.EnumerateDtoProperties(valueType.PropertyType, subValue);
            }
        }
        #endregion
        #endregion
    }
}