using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests
{
    public class PerControllerTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_generating_documents_per_controller_document_count_should_equal_controller_count()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();
            settings.PerController = true;

            // Act
            var documents = await GenerateDocumentsAsync(settings, typeof(Controller));
            var json = documents[0].ToJson();

            // Assert
            var operations = documents[0].Operations;
            Assert.True(operations.All(o => o.Operation.ActualParameters.All(p => p.Name != "api-version")));
        }
    }
}
