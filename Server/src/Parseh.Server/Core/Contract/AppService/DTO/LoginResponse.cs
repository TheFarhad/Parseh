using Newtonsoft.Json.Linq;
using Parseh.Server.Core.Domain.Aggregates.User.Entity;

namespace Parseh.Server.Core.Contract.AppService.DTO;

// TokenExpireDate
//      بهتر است فرستاده شود تا بتوانیم در سمت کلاینت هم چک کنیم که اگر منقضی شده بود
//      در خواست اضافه سمت سرور نفرستیم و مستقیما برای رفرش توکن اقدام کنیم
// TODO: اطلاعاتی نظیر نام و فامیلی هم برای نمایش به کلاینت ارسال شود
public record LoginResponse(string UserCode,/* string FirstName, string LastName,*/ string Token, DateTime TokenExpireDate);

public record RefreshTokenResponse(string UserCode,/* string FirstName, string LastName,*/ string Token, DateTime TokenExpireDate) : LoginResponse(UserCode,/* string FirstName, string LastName,*/ Token, TokenExpireDate)
{ }