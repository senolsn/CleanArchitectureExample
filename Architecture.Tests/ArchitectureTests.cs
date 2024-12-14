
using FluentAssertions;
using NetArchTest.Rules;

namespace Architecture.Tests
{
    public class ArchitectureTests
    {
        private const string DomainNamespace = "Domain";
        private const string ApplicationNamespace = "Application";
        private const string InfrastructureNamespace = "Infrastructure";
        private const string PresentationNamespace = "Presentation";
        private const string WebNamespace = "Web";

        [Fact]
        public void Domain_Should_Not_HaveDependencyOnOtherProjects()
        {
            //Arrange
            var assembly = typeof(Domain.AssemblyReference).Assembly;

            var otherProject = new[]
            {
                ApplicationNamespace,
                InfrastructureNamespace,
                PresentationNamespace,
                WebNamespace,
            };
            
            //Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProject)
                .GetResult();
            
            //Assert
            testResult.IsSuccessful.Should().BeTrue();

        }

        [Fact]
        public void Application_Should_Not_HaveDependencyOnOtherProjects()
        {
            //Arrange
            var assembly = typeof(Application.AssemblyReference).Assembly;

            var otherProject = new[]
            {
                InfrastructureNamespace,
                PresentationNamespace,
                WebNamespace,
            };

            //Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProject)
                .GetResult();

            //Assert
            testResult.IsSuccessful.Should().BeTrue();

        }

        [Fact]
        public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
        {
            //Arrange
            var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

            var otherProject = new[]
            {
                PresentationNamespace,
                WebNamespace,
            };

            //Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProject)
                .GetResult();

            //Assert
            testResult.IsSuccessful.Should().BeTrue();

        }

        [Fact]
        public void Presentation_Should_Not_HaveDependencyOnOtherProjects()
        {
            //Arrange
            var assembly = typeof(Presentation.AssemblyReference).Assembly;

            var otherProject = new[]
            {
                InfrastructureNamespace,
                WebNamespace,
            };

            //Act
            var testResult = Types
                .InAssembly(assembly)
                .ShouldNot()
                .HaveDependencyOnAll(otherProject)
                .GetResult();

            //Assert
            testResult.IsSuccessful.Should().BeTrue();

        }
    }
}
