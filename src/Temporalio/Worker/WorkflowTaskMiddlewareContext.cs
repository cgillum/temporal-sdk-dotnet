namespace Temporalio.Worker
{
    using System;
    using System.Collections.Generic;
    using Temporalio.Workflows;

    /// <summary>
    /// Context data that can be used to share data between middleware.
    /// </summary>
    public record WorkflowTaskMiddlewareContext(WorkflowInfo WorkflowInfo, WorkflowDefinition WorkflowDefinition)
    {
        private readonly Dictionary<string, object?> customProperties = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets a key/value collection that can be used to share data between middleware.
        /// </summary>
        public IReadOnlyDictionary<string, object?> CustomProperties => customProperties;

        /// <summary>
        /// Sets a property value to the context using the full name of the type as the key.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="value">The value of the property.</param>
        public void SetProperty<T>(T value)
        {
            string typeName = typeof(T).FullName ?? throw new InvalidOperationException("Cannot get the full name of the type.");
            SetProperty(typeName, value);
        }

        /// <summary>
        /// Sets a named property value to the context.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void SetProperty<T>(string key, T value)
        {
            customProperties[key] = value;
        }

        /// <summary>
        /// Gets a property value from the context using the full name of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <returns>The value of the property or <c>default(T)</c> if the property is not defined.</returns>
        public T? GetProperty<T>()
        {
            string typeName = typeof(T).FullName ?? throw new InvalidOperationException("Cannot get the full name of the type.");
            return GetProperty<T>(typeName);
        }

        /// <summary>
        /// Gets a named property value from the context.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="key">The name of the property value.</param>
        /// <returns>The value of the property or <c>default(T)</c> if the property is not defined.</returns>
        public T? GetProperty<T>(string key)
        {
            return customProperties.TryGetValue(key, out object? value) ? (T?)value : default;
        }
    }
}
