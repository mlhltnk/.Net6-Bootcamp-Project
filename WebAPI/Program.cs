using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependecyResolvers.Autofac;
using Core.DependecyResolvers;
using Core.Extensions;
using Core.Utilities.IOC;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();



//---JWT ���N YAPILAN TANIMLAMALAR
var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

var tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    });

builder.Services.AddDependecyResolvers(new ICoreModule[]             //COREMODULE; istedi�imiz kadar ekleyebilmek i�in yazd�k. CoreModule gibi istedi�imiz kadar modul olu�turup buraya ekleyebiliriz.
{
    new CoreModule()
});

//-------JWT ���N YAPILAN TANIMLAMALAR SON------





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




//.netcore �ncesinde program.cs; IOC Container altyap�s� sa�lamak i�in kullan�l�yordu. Autofac bize AOP imkan� sunuyor. Bu sebeple alttaki kodlar� Autofac'e ta��d�k

//builder.Services.AddSingleton<IProductService, ProductManager>();          //Bana arkaplanda bir referans olu�tur.(IOC Container)  //Birisi constructorda Iproductservice isterse ona arka planda productmanager olu�tur ve onu ver.
//builder.Services.AddSingleton<IProductDal, EfProductDal>();                //Birisi IproductDal isterse ona Efporductdal'� ver



//----.net core altyap�s�nda halihaz�rda varolan IOC container kulland�rmak yerine AUTOFAC kulland�rma yap�land�rmas�-----
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder2 => builder2.RegisterModule(new AutofacBusinessModule()));
//-----------------



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();            //JWT ���N TANIMLANDI

app.UseAuthorization();



app.Run();



