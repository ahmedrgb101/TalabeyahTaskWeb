using TalabeyahTaskWeb.Client.Components.Common;
using TalabeyahTaskWeb.Client.Infrastructure.ApiClient;
using MudBlazor;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TalabeyahTaskWeb.Client.Shared;
public static class Extensions
{
    public static string ToDescription(this Enum value)
    {
        if (value == null) return string.Empty;

        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
        return attribute == null ? r.Replace(value.ToString(), " ") : attribute.Description;
    }
}

public partial class CustomProblemDetails
{
    [Newtonsoft.Json.JsonProperty("errors", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public System.Collections.Generic.IDictionary<string, System.Collections.Generic.ICollection<string>> Errors { get; set; } = default!;
    public List<string> Messages { get; set; } = default!;

}

public static class ApiHelper
{
    public static async Task<T?> ExecuteCallGuardedAsync<T>(
        Func<Task<T>> call,
        ISnackbar snackbar,
        CustomValidation? customValidation = null,
        string? successMessage = null)
    {
        customValidation?.ClearErrors();
        try
        {
            var result = await call();

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                snackbar.Add(successMessage, Severity.Info);
            }

            return result;
        }
        catch (ApiException<HttpValidationProblemDetails> ex)
        {
            if (ex.Result.Errors is not null)
            {
                customValidation?.DisplayErrors(ex.Result.Errors);
                foreach (var errors in ex.Result.Errors?.Values)
                {
                    foreach (var error in errors)
                    {
                        snackbar.Add($"{error}", Severity.Error);
                    }
                }
            }
            else
            {
                snackbar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (ApiException<ErrorResult> ex)
        {
            snackbar.Add(ex.Result.Exception, Severity.Error);
        }
        catch (ApiException ex)
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();

            var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<CustomProblemDetails>(ex.Response);
            if (typedBody?.Errors is not null)
            {
                customValidation?.DisplayErrors(typedBody.Errors);
            }
            else if (typedBody?.Messages is not null)
            {
                foreach (string msg in typedBody.Messages)
                {
                    snackbar.Add(msg, Severity.Error);
                }
            }
            else
            {
                snackbar.Add(ex.Message, Severity.Error);
            }
        }
        catch (Exception ex)
        {
            snackbar.Add(ex.Message, Severity.Error);
        }

        return default;
    }

    public static async Task<bool> ExecuteCallGuardedAsync(
        Func<Task> call,
        ISnackbar snackbar,
        CustomValidation? customValidation = null,
        string? successMessage = null)
    {
        customValidation?.ClearErrors();
        try
        {
            await call();

            if (!string.IsNullOrWhiteSpace(successMessage))
            {
                snackbar.Add(successMessage, Severity.Success);
            }

            return true;
        }
        catch (ApiException<HttpValidationProblemDetails> ex)
        {
            if (ex.Result.Errors is not null)
            {
                customValidation?.DisplayErrors(ex.Result.Errors);
                foreach (var errors in ex.Result.Errors?.Values)
                {
                    foreach (var error in errors)
                    {
                        snackbar.Add($"{error}", Severity.Error);
                    }
                }
            }
            else
            {
                snackbar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (ApiException<ErrorResult> ex)
        {
            snackbar.Add(ex.Result.Exception, Severity.Error);
        }

        return false;
    }
}