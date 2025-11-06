using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using POE_part2.Controllers;
using POE_part2.Models;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Xunit;

namespace POE_part2.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<ILogger<HomeController>> _loggerMock;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_loggerMock.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_UserRedirectsToLoginOnSuccess()
        {
            // Arrange
            var addUser = new register
            {
                username = "testuser",
                email = "test@example.com",
                password = "password",
                role = "Lecturer"
            };

            // Simulate the user being successfully registered
            // Assuming the insert_user method returns "done" on success
            addUser.insert_user = (name, email, role, password) => "done";

            // Act
            var result = _controller.Register_user(addUser) as RedirectToActionResult;

            // Assert
            Assert.Equal("Login", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public void Login_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Dashboard_ReturnsViewResult()
        {
            // Act
            var result = _controller.Dashboard();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_UserRedirectsToDashboardOnSuccess()
        {
            // Arrange
            var user = new check_login
            {
                email = "test@example.com",
                role = "Lecturer",
                Password = "password"
            };

            // Simulate successful login
            user.login_user = (email, role, password) => "found";

            // Act
            var result = _controller.login_user(user) as RedirectToActionResult;

            // Assert
            Assert.Equal("Dashboard", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public void Claim_sub_ReturnsDashboardOnSuccess()
        {
            // Arrange
            var insert = new Claim
            {
                user_email = "test@example.com",
                hours_worked = "10",
                hour_rate = "100",
                description = "Worked on project"
            };

            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("test.pdf");
            file.Setup(f => f.Length).Returns(1024);

            // Simulate the claim being inserted successfully
            insert.insert_claim = (email, hours, rate, desc, filename) => "done";

            // Act
            var result = _controller.Claim_sub(file.Object, insert) as RedirectToActionResult;

            // Assert
            Assert.Equal("Dashboard", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }

        [Fact]
        public void View_claims_ReturnsViewResult()
        {
            // Act
            var result = _controller.view_claims();

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = result.Model as get_claims;
            Assert.NotNull(model);
        }

        [Fact]
        public void Approve_claims_ReturnsViewResult()
        {
            // Act
            var result = _controller.approve_claims();

            // Assert
            Assert.IsType<ViewResult>(result);
            var model = result.Model as get_claims;
            Assert.NotNull(model);
        }
    }
}

