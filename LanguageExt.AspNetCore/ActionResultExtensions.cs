using LanguageExt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace NickDarvey.LanguageExt.AspNetCore
{
    public static class ActionResultExtensions
    {
        private static IActionResult GetGenericErrorResult(Exception ex) =>
            ex is AggregateException aggregate ? GetInternalServerErrorResult(aggregate.InnerExceptions)
            : GetInternalServerErrorResult(ex);

        private static IActionResult GetGenericErrorResult<T>(T error) =>
            error is Exception ex ? GetGenericErrorResult(ex)
            : GetInternalServerErrorResult(error);

        private static IActionResult GetGenericSuccessResult<T>(T value) =>
            value is Unit ? GetNoContentResult()
            : new OkObjectResult(value);

        private static IActionResult GetInternalServerErrorResult(object value) =>
            new ObjectResult(value) { StatusCode = StatusCodes.Status500InternalServerError };

        private static IActionResult GetNoContentResult() => new NoContentResult();

        private static IActionResult GetNotFoundResult() => new NotFoundResult();

        /// <summary>
        /// Converts a <see cref="Try{A}"/> to an <see cref="IActionResult"/>.
        /// Default success case conversion will convert units to NoContent and all other values to Ok.
        /// Default failure case conversion will convert exceptions to an InternalServerError.
        /// </summary>
        /// <typeparam name="A">The <see cref="Try{A}"/> parameter type.</typeparam>
        /// <param name="this">The <see cref="Try{A}"/> to convert to an <see cref="IActionResult"/>.</param>
        /// <param name="Succ">Optionally, a custom conversion for the success case.</param>
        /// <param name="Fail">Optionally, a custom conversion for the failure case.</param>
        /// <returns>An <see cref="IActionResult"/> which can be returned from a controller.</returns>
        public static IActionResult ToActionResult<T>(this Try<T> @this, Func<T, IActionResult> Succ = null, Func<Exception, IActionResult> Fail = null) =>
            @this.Match(Succ: Succ ?? GetGenericSuccessResult, Fail: Fail ?? GetGenericErrorResult);

        /// <summary>
        /// Converts a <see cref="TryAsync{A}"/> to an <see cref="IActionResult"/>.
        /// Default success case conversion will convert units to NoContent and all other values to Ok.
        /// Default failure case conversion will convert exceptions to an InternalServerError.
        /// </summary>
        /// <typeparam name="A">The <see cref="TryAsync{A}"/> parameter type.</typeparam>
        /// <param name="this">The <see cref="TryAsync{A}"/> to convert to an <see cref="IActionResult"/>.</param>
        /// <param name="Succ">Optionally, a custom conversion for the success case.</param>
        /// <param name="Fail">Optionally, a custom conversion for the failure case.</param>
        /// <returns>An <see cref="IActionResult"/> which can be returned from a controller.</returns>
        public static Task<IActionResult> ToActionResult<T>(this TryAsync<T> @this, Func<T, IActionResult> Succ = null, Func<Exception, IActionResult> Fail = null) =>
            @this.Match(Succ: Succ ?? GetGenericSuccessResult, Fail: Fail ?? GetGenericErrorResult);

        /// <summary>
        /// Converts a <see cref="Either{L, R}"/> to an <see cref="IActionResult"/>.
        /// Default right case conversion will convert units to NoContent and all other values to Ok.
        /// Default left case conversion will convert values to an InternalServerError.
        /// </summary>
        /// <typeparam name="L">The left <see cref="Either{L, R}"/> parameter type.</typeparam>
        /// <typeparam name="R">The right <see cref="Either{L, R}"/> parameter type.</typeparam>
        /// <param name="this">The <see cref="Either{L, R}"/> to convert to an <see cref="IActionResult"/>.</param>
        /// <param name="Right">Optionally, a custom conversion for the right case.</param>
        /// <param name="Left">Optionally, a custom conversion for the left case.</param>
        /// <returns>An <see cref="IActionResult"/> which can be returned from a controller.</returns>
        public static IActionResult ToActionResult<L, R>(this Either<L, R> @this, Func<R, IActionResult> Right = null, Func<L, IActionResult> Left = null) =>
            @this.Match(Right: Right ?? GetGenericSuccessResult, Left: Left ?? GetGenericErrorResult);

        /// <summary>
        /// Converts a <see cref="EitherAsync{L, R}"/> to an <see cref="IActionResult"/>.
        /// Default right case conversion will convert units to NoContent and all other values to Ok.
        /// Default left case conversion will convert values to an InternalServerError.
        /// </summary>
        /// <typeparam name="L">The left <see cref="EitherAsync{L, R}"/> parameter type.</typeparam>
        /// <typeparam name="R">The right <see cref="EitherAsync{L, R}"/> parameter type.</typeparam>
        /// <param name="this">The <see cref="EitherAsync{L, R}"/> to convert to an <see cref="IActionResult"/>.</param>
        /// <param name="Right">Optionally, a custom conversion for the right case.</param>
        /// <param name="Left">Optionally, a custom conversion for the left case.</param>
        /// <returns>An <see cref="IActionResult"/> which can be returned from a controller.</returns>
        public static Task<IActionResult> ToActionResult<L, R>(this EitherAsync<L, R> @this, Func<R, IActionResult> Right = null, Func<L, IActionResult> Left = null) =>
            @this.Match(Right: Right ?? GetGenericSuccessResult, Left: Left ?? GetGenericErrorResult);

        /// <summary>
        /// Converts a <see cref="Option{A}"/> to an <see cref="IActionResult"/>.
        /// Default some case conversion will convert units to NoContent and all other values to Ok.
        /// Default none case conversion will convert to a NotFound.
        /// </summary>
        /// <typeparam name="A">The <see cref="Option{A}"/> parameter type.</typeparam>
        /// <param name="this">The <see cref="Option{A}"/> to convert to an <see cref="IActionResult"/>.</param>
        /// <param name="Some">Optionally, a custom conversion for the some case.</param>
        /// <param name="None">Optionally, a custom conversion for the none case.</param>
        /// <returns>An <see cref="IActionResult"/> which can be returned from a controller.</returns>
        public static IActionResult ToActionResult<A>(this Option<A> @this, Func<A, IActionResult> Some = null, Func<IActionResult> None = null) =>
            @this.Match(Some: Some ?? GetGenericSuccessResult, None: None ?? GetNotFoundResult);

        /// <summary>
        /// Converts a <see cref="OptionAsync{A}"/> to an <see cref="IActionResult"/>.
        /// Default some case conversion will convert units to NoContent and all other values to Ok.
        /// Default none case conversion will convert to a NotFound.
        /// </summary>
        /// <typeparam name="A">The <see cref="OptionAsync{A}"/> parameter type.</typeparam>
        /// <param name="this">The <see cref="OptionAsync{A}"/> to convert to an <see cref="IActionResult"/>.</param>
        /// <param name="Some">Optionally, a custom conversion for the some case.</param>
        /// <param name="None">Optionally, a custom conversion for the none case.</param>
        /// <returns>An <see cref="IActionResult"/> which can be returned from a controller.</returns>
        public static Task<IActionResult> ToActionResult<A>(this OptionAsync<A> @this, Func<A, IActionResult> Some = null, Func<IActionResult> None = null) =>
            @this.Match(Some: Some ?? GetGenericSuccessResult, None: None ?? GetNotFoundResult);
    }
}
