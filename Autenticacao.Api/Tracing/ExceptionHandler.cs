﻿using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;

namespace Autenticacao.Api.Tracing
{
    /// <summary>
    /// Class in charge with handleing special exception cases 
    /// </summary>
    public class ExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
       {

            return HandleAsyncCore(context, cancellationToken);
        }

        public virtual Task HandleAsyncCore(ExceptionHandlerContext context,
                                   CancellationToken cancellationToken)
        {
            HandleCore(context);
            return Task.FromResult(0);
        }

        public virtual void HandleCore(ExceptionHandlerContext context)
        {
        }

        public virtual bool ShouldHandle(ExceptionHandlerContext context)
        {
            return context.CatchBlock.IsTopLevel;
        }
    }
}
