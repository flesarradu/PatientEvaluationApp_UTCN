using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace PatientEvaluationApp_UTCN.Lib
{
    public interface IDtoProperty<T>
    {
        #region Properties
        T Value { get; set; }
        #endregion
    }

public interface IDtoProperty : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Delegates/Events
        event PropertyChangedEventHandler ValueChanged;
        #endregion

        #region Properties
        Boolean HasChanges { get; }

        Boolean IsNotificationsSuspended { get; }

        List<ValidationAttribute> ValidationAttributes { get; }
        #endregion

        #region Public Methods
        void AddValidationAttributes(IEnumerable<ValidationAttribute> attributes, Object validationContext);

        void AcceptChanges();

        Boolean IsValid(Object validationContext);

        void SuspendNotifications();

        void ResumeNotifications();

        void RaisePropertyChanged([CallerMemberName] String propertyName = null);
        #endregion
    }
    public sealed class DtoProperty<T> : IDtoProperty, IDtoProperty<T>
    {
        #region Delegates/Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ValueChanged;
        #endregion

        #region Fields
        private Object validationContext;
        private T value;
        #endregion

        #region Constructors
        public DtoProperty(T value)
        {
            this.ValidationAttributes = new List<ValidationAttribute>();
            this.value = value;
        }
        #endregion

        #region Properties
        public T Value
        {
            get => this.value;
            set
            {
                if (Object.Equals(this.value, value)) return;

                this.value = value;
                this.HasChanges = true;

                this.RaisePropertyChanged(nameof(this.Value));
                this.ValueChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.Value)));
            }
        }

        [JsonIgnore]
        public Boolean HasChanges { get; private set; }

        [JsonIgnore]
        public Boolean IsNotificationsSuspended { get; private set; }

        [JsonIgnore]
        public String Error { get; private set; }

        public String this[String columnName]
        {
            get
            {
                this.Error = null;

                this.IsValid(this.validationContext);

                return this.Error;
            }
        }

        [JsonIgnore]
        public List<ValidationAttribute> ValidationAttributes { get; }
        #endregion

        #region Public Methods
        public void AddValidationAttributes(IEnumerable<ValidationAttribute> attributes, Object context)
        {
            this.ValidationAttributes.AddRange(attributes);
            this.validationContext = context;
        }

        public void AcceptChanges()
        {
            this.HasChanges = false;
        }

        public Boolean IsValid(Object context)
        {
            this.validationContext = context;

            foreach (var validationAttribute in this.ValidationAttributes)
            {
                if (this.validationContext != null)
                {
                    if (validationAttribute.GetValidationResult(this.value, new ValidationContext(this.validationContext)) != ValidationResult.Success)
                    {
                        this.Error = validationAttribute.FormatErrorMessage(null);
                        return false;
                    }
                }
                else
                {
                    var isValid = validationAttribute.IsValid(this.value);
                    if (!isValid)
                    {
                        this.Error = validationAttribute.FormatErrorMessage(null);
                        return false;
                    }
                }
            }

            return true;
        }

        public void SuspendNotifications()
        {
            this.IsNotificationsSuspended = true;
        }

        public void ResumeNotifications()
        {
            this.IsNotificationsSuspended = false;
        }

        public void RaisePropertyChanged([CallerMemberName] String propertyName = null)
        {
            try
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception)
            {
            }
        }

        public override String ToString()
        {
            return this.Value == null ? "null" : $"{this.Value}";
        }
        #endregion
    }
}
