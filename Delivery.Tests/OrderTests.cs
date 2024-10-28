using Delivery.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Delivery.Tests
{
    public class OrderTests
    {

        [Fact]
        public void Name_ValidatesCorrectly_WhenNameIsLettersAndSpaces()
        {
            // Arrange
            var order = new Order { Name = "Иван Иванов", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // Ожидаем, что ошибок не будет
        }

        [Fact]
        public void Name_ValidatesIncorrectly_WhenNameContainsNumbers()
        {
            // Arrange
            var order = new Order { Name = "Иван123", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // Ожидаем, что будут ошибки
            Assert.Contains(validationResults, result => result.ErrorMessage == "Имя может содержать только буквы и пробелы."); // Проверяем сообщение об ошибке
        }

        [Fact]
        public void Weight_ValidatesCorrectly_WhenWeightWithinRange()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 25.5 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // Ожидаем, что ошибок не будет
        }

        [Fact]
        public void Weight_ValidatesIncorrectly_WhenWeightBelowRange()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = -10 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // Ожидаем, что будут ошибки
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("Значение должно быть больше или равно 0.")); // Проверяем сообщение об ошибке
        }

        [Fact]
        public void Weight_ValidatesIncorrectly_WhenWeightAboveRange()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 105 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // Ожидаем, что будут ошибки
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("Значение должно быть меньше или равно 100.")); // Проверяем сообщение об ошибке
        }

        [Fact]
        public void District_ValidatesCorrectly_WhenDistrictIsLettersAndSpaces()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Центральный", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // Ожидаем, что ошибок не будет
        }

        [Fact]
        public void District_ValidatesIncorrectly_WhenDistrictContainsNumbers()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "123", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // Ожидаем, что будут ошибки
            Assert.Contains(validationResults, result => result.ErrorMessage == "Район может содержать только буквы и пробелы."); // Проверяем сообщение об ошибке
        }

        [Fact]
        public void DeliveryDateTime_ValidatesCorrectly_WhenDateTimeIsInFuture()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // Ожидаем, что ошибок не будет
        }

        [Fact]
        public void DeliveryDateTime_ValidatesIncorrectly_WhenDateTimeIsInPast()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(-1), Weight = 50 }; // Заполняем обязательные поля

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // Ожидаем, что будут ошибки
            Assert.Contains(validationResults, result => result.ErrorMessage == "Дата доставки должна быть в будущем."); // Проверяем сообщение об ошибке
        }
    }
}