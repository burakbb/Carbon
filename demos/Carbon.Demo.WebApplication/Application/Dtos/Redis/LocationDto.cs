using System;

namespace Carbon.Demo.WebApplication.Application.Dtos.Redis
{
    public class LocationDto
    {
        public LocationDto()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Address { get; set; }
    }
}
