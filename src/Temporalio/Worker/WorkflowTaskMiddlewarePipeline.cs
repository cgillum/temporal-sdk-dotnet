namespace Temporalio.Worker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// A function that runs in the task execution middleware pipeline.
    /// </summary>
    /// <param name="context">The <see cref="WorkflowTaskMiddlewareContext"/> for the task execution.</param>
    /// <returns>A task that represents the completion of the durable task execution.</returns>
    public delegate Task WorkflowTaskMiddlewareHandler(WorkflowTaskMiddlewareContext context);

    /// <summary>
    /// Middleware pipeline that wraps workflow task execution.
    /// </summary>
    internal class WorkflowTaskMiddlewarePipeline
    {
        private readonly IList<Func<WorkflowTaskMiddlewareHandler, WorkflowTaskMiddlewareHandler>> components =
            new List<Func<WorkflowTaskMiddlewareHandler, WorkflowTaskMiddlewareHandler>>();

        /// <summary>
        /// Runs the middleware pipeline handler with the specified context.
        /// </summary>
        /// <param name="context">Contextual information about the current workflow task.</param>
        /// <param name="handler">The final handler to execute at the end of the pipeline.</param>
        /// <returns>A task that completes when the full pipeline has completed.</returns>
        public Task RunAsync(WorkflowTaskMiddlewareContext context, WorkflowTaskMiddlewareHandler handler)
        {
            // Build the delegate chain
            foreach (Func<WorkflowTaskMiddlewareHandler, WorkflowTaskMiddlewareHandler> component in components.Reverse())
            {
                handler = component(handler);
            }

            return handler(context);
        }

        /// <summary>
        /// Adds a new middleware component to the pipeline.
        /// </summary>
        /// <param name="middleware">The middleware component to add.</param>
        public void Add(Func<WorkflowTaskMiddlewareContext, Func<Task>, Task> middleware)
        {
            this.components.Add(next =>
            {
                return context =>
                {
                    Task SimpleNext() => next(context);
                    return middleware(context, SimpleNext);
                };
            });
        }
    }
}
