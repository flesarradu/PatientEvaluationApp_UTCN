using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PatientEvaluationApp_UTCN.Lib
{
    public interface ILocalStorageService
    {
        #region Public Methods
        T GetValue<T>() where T : class;

        T GetValue<T>(String name);

        void SetValue<T>(T data) where T : class;

        void SetValue(String name, Object data);
        #endregion
    }

    public class LocalStorageService : ILocalStorageService
    {
        #region Fields
        private JObject json;
        #endregion

        #region Constructors
        public LocalStorageService()
        {
            this.GetStorageData();
        }
        #endregion

        #region Public Methods
        public T GetValue<T>() where T : class
        {
            return this.GetValue<T>(typeof(T).Name);
        }

        public T GetValue<T>(String name)
        {
            var value = this.json.GetValue(name);
            var result = value == null ? default : value.ToObject<T>();
            return result;
        }

        public void SetValue<T>(T data) where T : class
        {
            this.SetValue(typeof(T).Name, data);
        }

        public void SetValue(String name, Object data)
        {
            if (this.json.ContainsKey(name))
            {
                this.json[name] = data == null ? null : JToken.FromObject(data);
            }
            else
            {
                this.json.Add(name, data == null ? null : JToken.FromObject(data));
            }

            this.SetStorageData();
        }
        #endregion

        #region Private Methods
        private void SetStorageData()
        {
            try
            {
                var jsonFile = JsonConvert.SerializeObject(this.json);
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(basePath, "localStorage.json");
                File.WriteAllText(filePath, jsonFile);
            }
            catch (Exception)
            {
            }
        }

        private void GetStorageData()
        {
            try
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var filePath = Path.Combine(basePath, "localStorage.json");
                if (File.Exists(filePath))
                {
                    var jsonFile = File.ReadAllText(filePath);
                    this.json = JObject.Parse(jsonFile);
                }
                else
                {
                    this.json = new JObject();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}
