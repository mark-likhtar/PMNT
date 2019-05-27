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

        public void UpdateInstance(ManagementBaseObject instance, T instanceUpdate)
        {
            var newInstance = _connection.CreateInstance();

            foreach (var prop in _connection.Properties)
            {
                var attributeProperty = WMIPropertyAttribute.GetAttributeField(instanceUpdate, prop.Name);
                var value = attributeProperty.Property.GetValue(instanceUpdate);

                if (!CheckPropertyIsDefault(attributeProperty, instanceUpdate))
                {
                    newInstance[prop.Name] = value;
                }

                Console.WriteLine($"{prop.Name}: {newInstance[prop.Name]}");
            }

            Console.WriteLine("Ready for put");
            Console.ReadKey();
            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.UpdateOnly;
                newInstance.Put(options);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        public void AddInstance(T instance)
        {
            var newInstance = _connection.CreateInstance();

            foreach (var prop in _connection.Properties)
            {
                var attributeProperty = WMIPropertyAttribute.GetAttributeField<T>(instance, prop.Name);
                var value = attributeProperty.Property.GetValue(instance);

                if (!CheckPropertyIsDefault(attributeProperty, instance))
                {
                    newInstance[prop.Name] = value;
                }

                Console.WriteLine($"{prop.Name}: {newInstance[prop.Name]}");
            }

            Console.WriteLine("Ready for put");
            Console.ReadKey();
            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.CreateOnly;
                newInstance.Put(options);
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
        }

        public ManagementObjectCollection GetInstances()
        {
            return _connection.GetInstances();
        }

        private bool CheckPropertyIsDefault(AttributeProperty<WMIPropertyAttribute> attributeProperty, T instance)
        {
            var value = attributeProperty.Property.GetValue(instance);

            return (attributeProperty.Attribute.Type == typeof(int) && (int)value == default) ||
                    (attributeProperty.Attribute.Type == typeof(string) && (string)value == default) ||
                    (attributeProperty.Attribute.Type == typeof(bool) && (bool)value == default) ||
                    (attributeProperty.Attribute.Type == typeof(DateTime) && (DateTime)value == default);
        }
    }
}
