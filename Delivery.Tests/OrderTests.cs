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
            var order = new Order { Name = "���� ������", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // �������, ��� ������ �� �����
        }

        [Fact]
        public void Name_ValidatesIncorrectly_WhenNameContainsNumbers()
        {
            // Arrange
            var order = new Order { Name = "����123", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // �������, ��� ����� ������
            Assert.Contains(validationResults, result => result.ErrorMessage == "��� ����� ��������� ������ ����� � �������."); // ��������� ��������� �� ������
        }

        [Fact]
        public void Weight_ValidatesCorrectly_WhenWeightWithinRange()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 25.5 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // �������, ��� ������ �� �����
        }

        [Fact]
        public void Weight_ValidatesIncorrectly_WhenWeightBelowRange()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = -10 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // �������, ��� ����� ������
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("�������� ������ ���� ������ ��� ����� 0.")); // ��������� ��������� �� ������
        }

        [Fact]
        public void Weight_ValidatesIncorrectly_WhenWeightAboveRange()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 105 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // �������, ��� ����� ������
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("�������� ������ ���� ������ ��� ����� 100.")); // ��������� ��������� �� ������
        }

        [Fact]
        public void District_ValidatesCorrectly_WhenDistrictIsLettersAndSpaces()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "�����������", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // �������, ��� ������ �� �����
        }

        [Fact]
        public void District_ValidatesIncorrectly_WhenDistrictContainsNumbers()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "123", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // �������, ��� ����� ������
            Assert.Contains(validationResults, result => result.ErrorMessage == "����� ����� ��������� ������ ����� � �������."); // ��������� ��������� �� ������
        }

        [Fact]
        public void DeliveryDateTime_ValidatesCorrectly_WhenDateTimeIsInFuture()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(1), Weight = 50 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.Empty(validationResults); // �������, ��� ������ �� �����
        }

        [Fact]
        public void DeliveryDateTime_ValidatesIncorrectly_WhenDateTimeIsInPast()
        {
            // Arrange
            var order = new Order { Name = "Test Name", District = "Test District", DeliveryDateTime = DateTime.Now.AddDays(-1), Weight = 50 }; // ��������� ������������ ����

            // Act
            var validationContext = new ValidationContext(order);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(order, validationContext, validationResults, true);

            // Assert
            Assert.NotEmpty(validationResults); // �������, ��� ����� ������
            Assert.Contains(validationResults, result => result.ErrorMessage == "���� �������� ������ ���� � �������."); // ��������� ��������� �� ������
        }
    }
}