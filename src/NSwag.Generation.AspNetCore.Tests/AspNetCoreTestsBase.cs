using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore.Tests.Web;

namespace NSwag.Generation.AspNetCore.Tests
{
    public class AspNetCoreTestsBase : IDisposable
    {
        public AspNetCoreTestsBase()
        {
            TestServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        protected TestServer TestServer { get; }

        protected async Task<OpenApiDocument> GenerateDocumentAsync(AspNetCoreOpenApiDocumentGeneratorSettings settings, params Type[] controllerTypes)
        {
            var generator = new AspNetCoreOpenApiDocumentGenerator(settings);
            var provider = TestServer.Host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            var controllerTypeNames = controllerTypes.Select(t => t.FullName);

            var groups = new ApiDescriptionGroupCollection(provider.ApiDescriptionGroups.Items
                .Select(i => new ApiDescriptionGroup(i.GroupName, i.Items.Where(u => controllerTypeNames.Contains(((ControllerActionDescriptor)u.ActionDescriptor).ControllerTypeInfo.FullName)).ToList())).ToList(),
                provider.ApiDescriptionGroups.Version);

            var document = await generator.GenerateAsync(groups);
            return document;
        }

        protected async Task<List<OpenApiDocument>> GenerateDocumentsAsync(AspNetCoreOpenApiDocumentGeneratorSettings settings, params Type[] controllerTypes)
        {
            var generator = new AspNetCoreOpenApiDocumentGenerator(settings);
            var provider = TestServer.Host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            var controllerTypeNames = controllerTypes.Select(t => t.FullName);
            //var groupByType = new List<>();


            var groups = new ApiDescriptionGroupCollection(provider.ApiDescriptionGroups.Items
                    .Select(i => new ApiDescriptionGroup(i.GroupName, i.Items.Where(u => controllerTypeNames.Contains(((ControllerActionDescriptor)u.ActionDescriptor).ControllerTypeInfo.BaseType?.FullName)).ToList())).ToList(),
                provider.ApiDescriptionGroups.Version);
            var documents = new List<OpenApiDocument>();

            foreach (var group in groups.Items[0].Items)
            {
                var test = new ApiDescriptionGroup(group.ActionDescriptor.DisplayName,new List<ApiDescription>{@group});
                var bust = new List<ApiDescriptionGroup> {test};
                var single = new ApiDescriptionGroupCollection(bust, 0);
                documents.Add(await generator.GenerateAsync(single));
            }
            
            return documents;
        }

        public void Dispose()
        {
            TestServer.Dispose();
        }
    }
}