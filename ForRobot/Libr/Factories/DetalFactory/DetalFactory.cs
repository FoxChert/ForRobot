using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

using ForRobot.Libr.Json.Schemas;
using ForRobot.Libr.Services.Providers;
using ForRobot.Models.Detals;

namespace ForRobot.Libr.Factories.DetalFactory
{
    /// <summary>
    /// Фабрика для создания и сериализации/десериализации деталей
    /// </summary>
    public class DetalFactory : IDetalFactory, IDisposable
    {
        private readonly IConfigurationProvider _configProvider;
        private readonly IJsonSchemaProvider _jsonSchemaProvider;

        /// <summary>
        /// Инициализирует новый экземпляр класса DetalFactory
        /// </summary>
        /// <param name="configProvider">Провайдер вывода свойств узлов конфигурации</param>
        /// <param name="jsonSchemaProvider">Провайдер вывода JSON схем</param>
        /// <exception cref="ArgumentNullException">Если любой из параметров равен null</exception>
        public DetalFactory(IConfigurationProvider configProvider, IJsonSchemaProvider jsonSchemaProvider)
        {
            this._configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
            this._jsonSchemaProvider = jsonSchemaProvider ?? throw new ArgumentNullException(nameof(jsonSchemaProvider));
        }

        #region Private functions

        /// <summary>
        /// Определение DetalType на основе generic типа
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <returns>Соответствующий DetalType</returns>
        /// <exception cref="NotSupportedException">Если тип не поддерживается</exception>
        private DetalType GetDetalTypeFromGenericType<T>() where T : Detal
        {
            Type targetType = typeof(T);

            switch (targetType)
            {
                case Type plate when plate == typeof(Plita):
                    return DetalType.Plita;

                default:
                    throw new NotSupportedException($"Тип {targetType.Name} не поддерживается фабрикой");
            }
        }

        /// <summary>
        /// Создает деталь типа <see cref="DetalType.Plita"/> с параметрами из конфигурации
        /// </summary>
        /// <returns>Новая деталь типа Plita</returns>
        private Plita CreatePlita()
        {
            var plateConfig = _configProvider.GetPlitaConfig();

            if (plateConfig == null)
                throw new InvalidOperationException($"Конфигурация для детали {DetalType.Plita} не найдена");

            return new Plita()
            {
                ReverseDeflection = plateConfig.ReverseDeflection,
                PlateWidth = plateConfig.PlateWidth,
                PlateLength = plateConfig.PlateLength,
                PlateThickness = plateConfig.PlateThickness,
                PlateBevelToLeft = plateConfig.PlateBevelToLeft,
                PlateBevelToRight = plateConfig.PlateBevelToRight,

                RibsHeight = plateConfig.RibsHeight,
                RibsThickness = plateConfig.RibsThickness,
                RibsCount = plateConfig.RibsCount,
                DistanceToFirstRib = plateConfig.DistanceToFirstRib,
                DistanceBetweenRibs = plateConfig.DistanceBetweenRibs,
                RibsIdentToLeft = plateConfig.RibsIdentToLeft,
                RibsIdentToRight = plateConfig.RibsIdentToRight,
                WeldsDissolutionLeft = plateConfig.WeldsDissolutionLeft,
                WeldsDissolutionRight = plateConfig.WeldsDissolutionRight,


            };
        }

        /// <summary>
        /// Десериализует строку JSON в объект типа <see cref="Plita"/>
        /// </summary>
        /// <param name="jsonString">Строка JSON для десериализации</param>
        /// <param name="settings">Настройки сериализатора</param>
        /// <returns>Десериализованный объект <see cref="Plita"/></returns>
        private Plita DeserializePlate(string jsonString, JsonSerializerSettings settings)
        {
            if (string.IsNullOrEmpty(jsonString))
                return this.CreateDetal<Plita>(DetalType.Plita);
            else
                return JsonConvert.DeserializeObject<Plita>(jsonString, settings);
        }

        /// <summary>
        /// Обработчик вызова ошибки сериализации/десериализации
        /// </summary>
        /// <param name="sender">Источник события</param>
        /// <param name="e">Аргументы события ошибки</param>
        private void HandleSerializeringError(object sender, ErrorEventArgs e)
        {
            if (e?.ErrorContext == null)
                return;

            string message = string.Empty;
            var obj = e.CurrentObject as Detal;

            if (obj == null)
                message = e.ErrorContext.Error.Message;
            else
                message = string.Format("Ошибка сериализации/десериализации объекта {0}: {1}", obj.GetType(), e.ErrorContext.Error.Message);

            e.ErrorContext.Handled = true;
            throw new JsonSerializationException(message);
        }

        #endregion Private functions

        #region Public functions

        /// <summary>
        /// Создание детали указанного типа, определяемого по generic параметру
        /// </summary>
        /// <typeparam name="T">Тип детали для создания</typeparam>
        /// <returns>Созданная деталь указанного типа</returns>
        /// <exception cref="NotSupportedException">Если тип детали не поддерживается</exception>
        /// <exception cref="InvalidOperationException">Если произошла ошибка при создании детали</exception>
        public T CreateDetal<T>() where T : Detal
        {
            DetalType detalType = GetDetalTypeFromGenericType<T>();
            return (T)CreateDetal(detalType);
        }

        /// <summary>
        /// Создание детали указанного типа <see cref="T"/>
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <param name="type">Тип детали для создания</param>
        /// <returns>Созданная деталь указанного типа</returns>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        public T CreateDetal<T>(DetalType type) where T : Detal
        {
            return (T)CreateDetal(type);
        }

        /// <summary>
        /// Создание детали указанного типа
        /// </summary>
        /// <param name="type">Тип детали для создания</param>
        /// <returns>Созданная деталь</returns>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        /// <exception cref="InvalidOperationException">Если произошла ошибка при создании детали</exception>
        public Detal CreateDetal(DetalType type)
        {
            try
            {
                switch (type)
                {
                    case DetalType.Plita:
                        return CreatePlita();

                    default:
                        throw new ArgumentException($"Тип детали {DetalTypes.EnumToString(type)} не поддерживается", nameof(type));
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new InvalidOperationException($"Ошибка при создании детали типа {type}", ex);
            }
        }

        /// <summary>
        /// Десериализация и валидация JSON-строки в объект детали
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <param name="jsonString">Строка JSON для десериализации</param>
        /// <returns>Десериализованный объект детали</returns>
        /// <exception cref="ArgumentNullException">Если jsonString равен null</exception>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        /// <exception cref="JsonSerializationException">Если произошла ошибка десериализации</exception>
        /// <exception cref="JsonSchemaValidationException">Если JSON не прошел валидацию по схеме</exception>
        public T Deserialize<T>(string jsonString) where T : Detal
        {
            return (T)Deserialize(jsonString);
        }

        /// <summary>
        /// Десериализация и валидация строки JSON в объект детали
        /// </summary>
        /// <param name="jsonString">Строка JSON для десериализации</param>
        /// <returns>Десериализованный объект детали</returns>
        /// <exception cref="ArgumentNullException">Если jsonString равен null</exception>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        /// <exception cref="JsonSerializationException">Если произошла ошибка десериализации</exception>
        /// <exception cref="JsonSchemaValidationException">Если JSON не прошел валидацию по схеме</exception>
        public Detal Deserialize(string jsonString)
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentException("Строка JSON не может быть пустой", nameof(jsonString));
            
            var settings = new JsonSerializerSettings()
            {
                Error = HandleSerializeringError
            };

            try
            {
                var jsonObject = JObject.Parse(jsonString);
                var detalTypeToken = jsonObject[nameof(Detal.DetalType)];

                if (detalTypeToken == null)
                    throw new ArgumentException("JSON должен содержать поле DetalType", nameof(jsonString));

                string detalType = detalTypeToken.ToString();

                switch (detalType)
                {
                    case DetalTypes.Plita:
                        if (!this.ValidationJsonString<Plita>(jsonString))
                            return this.CreateDetal<Plita>();

                        return this.DeserializePlate(jsonString, settings);

                    default:
                        throw new ArgumentException($"Тип детали {detalType} не поддерживается", detalType);
                }
            }
            catch (JsonReaderException ex)
            {
                throw new JsonSerializationException("Некорректный формат JSON", ex);
            }
        }

        /// <summary>
        /// Сериализация детали в JSON-строку
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <param name="detal">Деталь для сериализации</param>
        /// <param name="contractResolver">Резолвер контрактов (опционально)</param>
        /// <returns>Строка JSON, представляющая деталь</returns>
        /// <exception cref="ArgumentNullException">Если detal равен null</exception>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        public string Serialize<T>(T detal, IContractResolver contractResolver = null) where T : Detal
        {
            return Serialize(detal as Detal, contractResolver);
        }

        /// <summary>
        /// Сериализация детали в JSON-строку
        /// </summary>
        /// <param name="detal">Деталь для сериализации</param>
        /// <param name="contractResolver">Резолвер контрактов (опционально)</param>
        /// <returns>Строка JSON, представляющая деталь</returns>
        /// <exception cref="ArgumentNullException">Если detal равен null</exception>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        public string Serialize(Detal detal, IContractResolver contractResolver = null)
        {
            if (detal == null)
                throw new ArgumentNullException(nameof(detal));
            
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver ?? new DefaultContractResolver(),
                Error = HandleSerializeringError
            };

            string jsonString = string.Empty;
            try
            {
                switch (detal.DetalType)
                {
                    case DetalTypes.Plita:
                        this.ValidationJsonString<Plita>(jsonString);
                        jsonString = JsonConvert.SerializeObject(detal, settings);
                        break;

                    default:
                        string typeName = detal.DetalType ?? detal.GetType().Name;
                        throw new ArgumentException($"Тип детали {typeName} не поддерживается", nameof(detal));
                }
            }
            catch (JsonSerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сериализации детали типа {detal.DetalType}", ex);
            }
            return jsonString;
        }

        /// <summary>
        /// Валидация JSON-строки по схеме для указанного типа детали
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <param name="jsonString">Строка JSON для валидации</param>
        /// <returns>True, если валидация прошла успешно</returns>
        /// <exception cref="ArgumentNullException">Если jsonString равен null</exception>
        /// <exception cref="JsonSchemaValidationException">Если JSON не прошел валидацию</exception>
        public bool ValidationJsonString<T>(string jsonString) where T : Detal
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentException("Строка JSON не может быть пустой", nameof(jsonString));
            
            string schemaTitle = "Unknown Schema";
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);
                JSchema schema = _jsonSchemaProvider.GetPlitaSchema();
                schemaTitle = schema.Title ?? schemaTitle;
                var validationErrors = new List<ValidationErrorInfo>();
                
                // Сбор ошибок валидации
                jsonObject.Validate(schema, (sender, args) =>
                {
                    validationErrors.Add(new ValidationErrorInfo()
                    {
                        Message = args.Message,
                        Path = args.Path,
                        ErrorDetails = args
                    });
                });

                if (validationErrors.Count > 0)
                {
                    throw new JsonSchemaValidationException(schemaTitle, validationErrors, jsonString);
                }
            }
            catch (JsonReaderException ex)
            {
                var errors = new List<ValidationErrorInfo>()
                {
                    new ValidationErrorInfo()
                    {
                        Message = $"Deserialization failed: {ex.Message}",
                        Path = ex.Path
                    }
                };
                throw new JsonSchemaValidationException(schemaTitle, errors, jsonString, ex);
            }
            catch (JsonSchemaValidationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при валидации JSON для типа {typeof(T).Name}", ex);
            }

            return true;
        }

        /// <summary>
        /// Очищение кэша провайдеров конфигурации и схем
        /// </summary>
        public void ClearCache()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DetalFactory));

            try
            {
                if (_configProvider is ForRobot.Libr.Configuration.CachedConfigurationProvider cachedConfigProvider)
                {
                    cachedConfigProvider.ClearCache();
                }

                if (_jsonSchemaProvider is ForRobot.Libr.Json.Schemas.CachedJsonSchemaProvider cachedJsonProvider)
                {
                    cachedJsonProvider.ClearCache();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при очистке кэша: {ex.Message}");
            }
        }

        #endregion Public functions

        #region Implementations of IDisposable

        private bool _disposed = false;

        ~DetalFactory() => this.Dispose();

        /// <summary>
        /// Освобождает ресурсы, используемые фабрикой
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Освобождение ресурсов, используемых фабрикой
        /// </summary>
        /// <param name="disposing">True, если вызвано из Dispose(), false, если из финализатора</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                ClearCache();
                _disposed = true;
            }
        }

        #endregion
    }
}
