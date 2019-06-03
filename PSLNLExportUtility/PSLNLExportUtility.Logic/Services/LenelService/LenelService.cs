using PSLNLExportUtility.Infrastructure.Logging;
using PSLNLExportUtility.Logic.Attributes;
using PSLNLExportUtility.Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PSLNLExportUtility.Logic.Services.LenelService
{
    public class LenelService<T>
    {
        private readonly ManagementClass _connection;

        public static readonly Logger _logger = new Logger();

        public LenelService(string className)
        {
            _connection = this.GetManagementClass(className);
        }

        public ManagementClass GetManagementClass(string className)
        {
            var scope = new ManagementScope("\\root\\OnGuard");
            var path = new ManagementPath(className);
            var options = new ObjectGetOptions(null, TimeSpan.MaxValue, true);

            return new ManagementClass(scope, path, options);
        }

        public bool UpdateInstance(ManagementBaseObject instance, T instanceUpdate)
        {
            var newInstance = instance as ManagementObject;
            var props = new List<string>();

            foreach (var prop in _connection.Properties)
            {
                var attributeProperty = WMIPropertyAttribute.GetAttributeField(instanceUpdate, prop.Name);
                if (attributeProperty == null)
                {
                    continue;
                }

                var value = attributeProperty.Property.GetValue(instanceUpdate);

                if (!CheckPropertyIsDefault(attributeProperty, instanceUpdate))
                {
                    newInstance[prop.Name] = value;
                }

                props.Add($"{prop.Name}: {newInstance[prop.Name]}");
            }

            _logger.Info($"[UPDATE] {string.Join(", ", props)}");
            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.UpdateOnly;
                newInstance.Put(options);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
            return true;
        }

        public bool AddInstance(T instance)
        {
            var newInstance = _connection.CreateInstance();
            var props = new List<string>();

            foreach (var prop in _connection.Properties)
            {
                var attributeProperty = WMIPropertyAttribute.GetAttributeField<T>(instance, prop.Name);
                if (attributeProperty == null)
                {
                    continue;
                }

                var value = attributeProperty.Property.GetValue(instance);

                if (!CheckPropertyIsDefault(attributeProperty, instance))
                {
                    newInstance[prop.Name] = value;
                }

                props.Add($"{prop.Name}: {newInstance[prop.Name]}");
            }

            _logger.Info($"[INSERT] {string.Join(", ", props)}");

            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.CreateOnly;
                newInstance.Put(options);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
            return true;
        }

        public ManagementObjectCollection GetInstances()
        {
            return _connection.GetInstances();
        }

        private bool CheckPropertyIsDefault(AttributeProperty<WMIPropertyAttribute> attributeProperty, T instance)
        {
            var value = attributeProperty.Property.GetValue(instance);

            return (attributeProperty.Attribute.Type == typeof(int) && (int)value == default) ||
                    (attributeProperty.Attribute.Type == typeof(string) && string.IsNullOrEmpty((string)value)) ||
                    (attributeProperty.Attribute.Type == typeof(bool) && (bool)value == default) ||
                    (attributeProperty.Attribute.Type == typeof(DateTime) && (DateTime)value == default);
        }
    }
}
