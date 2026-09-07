using EventParkingReservation.DTOs.Customer;
using EventParkingReservation.Models;
using EventParkingReservation.Repositories.Interfaces;
using EventParkingReservation.Services.Interfaces;

namespace EventParkingReservation.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(
            ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerResponseDto>>
            GetAllAsync(string? search)
        {
            var customers =
                await _repository.GetAllAsync(search);

            return customers.Select(Map);
        }

        public async Task<CustomerResponseDto>
            GetByIdAsync(int id)
        {
            var customer =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Customer not found.");

            return Map(customer);
        }

        public async Task<CustomerResponseDto>
            UpdateAsync(
                int id,
                UpdateCustomerDto dto)
        {
            var customer =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Customer not found.");

            customer.Name =
                dto.Name.Trim();

            customer.PhoneNumber =
                dto.PhoneNumber.Trim();

            await _repository.UpdateAsync(
                customer);

            return Map(customer);
        }

        public async Task DeactivateAsync(int id)
        {
            var customer =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Customer not found.");

            customer.Status = "Inactive";

            await _repository.UpdateAsync(
                customer);
        }

        public async Task ReactivateAsync(int id)
        {
            var customer =
                await _repository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException(
                    "Customer not found.");

            customer.Status = "Active";

            await _repository.UpdateAsync(
                customer);
        }

        private static CustomerResponseDto Map(
            Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId =
                    customer.CustomerId,

                Name =
                    customer.Name,

                Email =
                    customer.Email,

                PhoneNumber =
                    customer.PhoneNumber,

                Role =
                    customer.Role,

                Status =
                    customer.Status
            };
        }
    }
}