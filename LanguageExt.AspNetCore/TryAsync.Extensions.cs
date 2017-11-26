using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static class TryAsync
    {
        public static Task<IActionResult> ToActionResult<T>(this TryAsync<T> self) =>
            self.Match<T, IActionResult>(
                Succ: r => new OkObjectResult(r),
                Fail: ex => new InternalErrorResult(ex));
    }
}
