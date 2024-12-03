﻿namespace Api.Errors;

public class ApiResponse
{
    public ApiResponse(int statusCode, string? message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageForStatusCode(statusCode)!;
    }


    public int StatusCode { get; set; }
    public string Message { get; set; }

    private string? GetDefaultMessageForStatusCode(int statusCode)
    {
        // those phrases where taken from Sherlock for an ambient experience
        return statusCode switch
        {
            400 => "Oh, you've fumbled magnificently. A bad request, how utterly ordinary.",
            401 => "Unauthorized? My, my. Did you truly think you'd slip past unnoticed? How dull.",
            404 => "Ah, what you seek does not exist—or perhaps it's hiding, just out of reach. Intriguing, isn't it?",
            500 => "The machine falters, the gears grind to a halt. Chaos, as always, is the most delightful of companions.",
            _ => null
        };
    }
}