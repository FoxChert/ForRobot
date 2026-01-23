using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;

using ForRobot.Libr.Json.Schemas;
using ForRobot.Libr.Services.Providers;
using ForRobot.Models.Detals;
using ForRobot.Models.Welding;

namespace ForRobot.Libr.Factories.DetalFactory
{
    /// <summary>
    /// Фабрика для создания и сериализации/десериализации деталей
    /// </summary>
    public class DetalFactory : IDetalFactory, IDisposable
    {
        private readonly IJsonSchemaProvider _jsonSchemaProvider;
        private readonly IDetalProvider _detalProvider;

        private JsonSerializationException _serializationError;

        /// <summary>
        /// Событие возникновения ошибки валидации JSON-строки
        /// </summary>
        public static event Action<JsonSchemaValidationException> ValidatedError;

        /// <summary>
        /// Инициализирует новый экземпляр класса DetalFactory
        /// </summary>
        /// <param name="configProvider">Провайдер вывода свойств узлов конфигурации</param>
        /// <param name="jsonSchemaProvider">Провайдер вывода JSON схем</param>
        /// <exception cref="ArgumentNullException">Если любой из параметров равен null</exception>
        public DetalFactory(IDetalProvider detalProvider, IJsonSchemaProvider jsonSchemaProvider)
        {
            this._detalProvider = detalProvider ?? throw new ArgumentNullException(nameof(detalProvider));
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
                case Type plate when plate == typeof(Plate):
                    return DetalType.Plate;

                default:
                    throw new NotSupportedException($"Тип {targetType.Name} не поддерживается фабрикой");
            }
        }

        /// <summary>
        /// Десериализует строку JSON в объект типа <see cref="Plate"/>
        /// </summary>
        /// <param name="jsonString">Строка JSON для десериализации</param>
        /// <param name="settings">Настройки сериализатора</param>
        /// <returns>Десериализованный объект <see cref="Plate"/></returns>
        private Plate DeserializePlate(string jsonString, JsonSerializerSettings jsonSerializerSettings, JsonLoadSettings jsonLoadSettings = null)
        {
            if (string.IsNullOrEmpty(jsonString))
                return this.CreateDetal<Plate>(DetalType.Plate);
            else if (jsonLoadSettings == null)
                return JsonConvert.DeserializeObject<Plate>(jsonString, jsonSerializerSettings);
            else
                return JsonConvert.DeserializeObject<Plate>(JObject.Parse(jsonString, jsonLoadSettings).ToString(), jsonSerializerSettings);
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

            string message = "JSON Error\n" +
                             $"\tError: {e.ErrorContext.Error.Message}\n" +
                             $"\tPath: {e.ErrorContext.Path}\n" +
                             $"\tMember: {e.ErrorContext.Member}\n" +
                             $"\tOriginalObject: {e.ErrorContext.OriginalObject?.GetType().FullName}";

            this._serializationError = new JsonSerializationException(message);
            e.ErrorContext.Handled = true;
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
                    case DetalType.Plate:
                        return this._detalProvider.CreatePlita();

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
        public T Deserialize<T>(string jsonString, JsonSerializerSettings jsonSerializerSettings = null, JsonLoadSettings jsonLoadSettings = null) where T : Detal
        {
            return (T)Deserialize(jsonString, jsonSerializerSettings, jsonLoadSettings);
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
        public Detal Deserialize(string jsonString, JsonSerializerSettings jsonSerializerSettings = null, JsonLoadSettings jsonLoadSettings = null)
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentException("Строка JSON не может быть пустой", nameof(jsonString));

            var serSettings = jsonSerializerSettings ?? new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver(),
                ObjectCreationHandling = ObjectCreationHandling.Replace
            };

            var loadSettings = jsonLoadSettings ?? new JsonLoadSettings()
            {
                CommentHandling = CommentHandling.Ignore
            };

            JsonSchemaValidationException validationException = null;
            try
            {
                var jsonObject = JObject.Parse(jsonString);
                var detalTypeToken = jsonObject[nameof(Detal.DetalType)];

                if (detalTypeToken == null)
                    throw new ArgumentException("JSON должен содержать поле DetalType", nameof(jsonString));

                string detalType = detalTypeToken.ToString();

                switch (detalType)
                {
                    case DetalTypes.Plate:
                        bool isValid = this.ValidationJsonString<Plate>(jsonString, exception =>
                        {
                            validationException = exception;
                        });
                        if (!isValid)
                            return this.CreateDetal<Plate>();

                        return this.DeserializePlate(jsonString, serSettings, loadSettings);

                    default:
                        throw new ArgumentException($"Тип детали {detalType} не поддерживается", detalType);
                }
            }
            catch (JsonReaderException ex)
            {
                throw new JsonSerializationException("Некорректный формат JSON", ex);
            }
            finally
            {
                if (validationException != null)
                {
                    ValidatedError?.Invoke(validationException);
                }
            }
        }

        /// <summary>
        /// Сериализация детали в JSON-строку
        /// </summary>
        /// <typeparam name="T">Тип детали</typeparam>
        /// <param name="detal">Деталь для сериализации</param>
        /// <param name="contractResolver">Резолвер контрактов (опционально)</param>
        /// <param name="isValidate">Проводить ли валидацию JSON-строки (опционально)</param>
        /// <returns>Строка JSON, представляющая деталь</returns>
        /// <exception cref="ArgumentNullException">Если detal равен null</exception>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        public string Serialize<T>(T detal, JsonSerializerSettings jsonSerializerSettings = null, bool isValidate = false) where T : Detal
        {
            return Serialize(detal as Detal, jsonSerializerSettings, isValidate);
        }

        /// <summary>
        /// Сериализация детали в JSON-строку
        /// </summary>
        /// <param name="detal">Деталь для сериализации</param>
        /// <param name="contractResolver">Резолвер контрактов (опционально)</param>
        /// <param name="isValidate">Проводить ли валидацию JSON-строки (опционально)</param>
        /// <returns>Строка JSON, представляющая деталь</returns>
        /// <exception cref="ArgumentNullException">Если detal равен null</exception>
        /// <exception cref="ArgumentException">Если тип детали не поддерживается</exception>
        public string Serialize(Detal detal, JsonSerializerSettings jsonSerializerSettings = null, bool isValidate = false)
        {
            if (detal == null)
                throw new ArgumentNullException(nameof(detal));

            var settings = jsonSerializerSettings ?? new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver(),
                Error = HandleSerializeringError
            };
            
            string jsonString = string.Empty;
            JsonSchemaValidationException validationException = null;
            try
            {
                switch (detal.DetalType)
                {
                    case DetalTypes.Plate:
                        jsonString = JsonConvert.SerializeObject(detal, settings);

                        if (this._serializationError != null)
                            throw _serializationError;

                        if (isValidate)
                            this.ValidationJsonString<Plate>(jsonString, exception => 
                            {
                                validationException = exception;
                                throw validationException;
                            });
                        break;

                    default:
                        string typeName = detal.DetalType ?? detal.GetType().Name;
                        throw new ArgumentException($"Тип детали {typeName} не поддерживается", nameof(detal));
                }
            }
            //catch (JsonSerializationException ex)
            //{
            //    throw ex;
            //}
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при сериализации детали типа {detal.DetalType}", ex);
            }
            finally
            {
                this._serializationError = null;

                if (validationException != null)
                {
                    ValidatedError?.Invoke(validationException);
                }
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
        public bool ValidationJsonString<T>(string jsonString, Action<JsonSchemaValidationException> onValidationError = null) where T : Detal
        {
            if (jsonString == null)
                throw new ArgumentNullException(nameof(jsonString));

            if (string.IsNullOrEmpty(jsonString))
                throw new ArgumentException("Строка JSON не может быть пустой", nameof(jsonString));
            
            string schemaTitle = "Unknown Schema";
            try
            {
                JObject jsonObject = JObject.Parse(jsonString);
                JSchema schema = _jsonSchemaProvider.GetPlateSchema();
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
                    var exception = new JsonSchemaValidationException(schemaTitle, validationErrors, jsonString);
                    onValidationError?.Invoke(exception);
                    return false;
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
                var exception = new JsonSchemaValidationException(schemaTitle, errors, jsonString, ex);
                onValidationError?.Invoke(exception);
                return false;
            }
            catch (JsonSchemaValidationException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Ошибка при валидации JSON для типа {typeof(T).FullName}", ex);
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
                //if (_configProvider is ForRobot.Libr.Configuration.CachedConfigurationProvider cachedConfigProvider)
                //{
                //    cachedConfigProvider.ClearCache();
                //}

                if (this._detalProvider is ForRobot.Models.Detals.CachedDetalProvider cachedDetalProvider)
                {
                    cachedDetalProvider.ClearCache();
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
