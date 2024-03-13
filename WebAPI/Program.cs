using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.Abstract;
using Business.Concrete;
using Business.DependecyResolvers.Autofac;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Autofac,Ninject,Castlewindsor,strutureMap,Dryinject -->.netcore �ncesinde buras� IOC Container altyap�s� sa�lamak i�in kullan�l�yordu. Autofac bize AOP imkan� sunuyor. Bu sebeple buray� Autofac'e ta��d�k

//builder.Services.AddSingleton<IProductService, ProductManager>();          //Bana arkaplanda bir referans olu�tur.(IOC Container)  //Birisi constructorda Iproductservice isterse ona arka planda productmanager olu�tur ve onu ver.
//builder.Services.AddSingleton<IProductDal, EfProductDal>();               //Birisi IproductDal isterse ona Efporductdal'� ver



//----.net core altyap�s�nda default gelen IOC container yerine AUTOFAC kullanma yap�land�rmas�-----
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder2 => builder2.RegisterModule(new AutofacBusinessModule()));
//-----------------



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();



