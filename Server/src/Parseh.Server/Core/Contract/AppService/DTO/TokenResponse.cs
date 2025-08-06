namespace Parseh.Server.Core.Contract.AppService.DTO;

// TokenExpireDate
//      بهتر است فرستاده شود تا بتوانیم در سمت کلاینت هم چک کنیم که اگر منقضی شده بود
//      در خواست اضافه سمت سرور نفرستیم و مستقیما برای رفرش توکن اقدام کنیم
public sealed record TokenResponse(string UserCode, string Token, DateTime TokenExpireDate);