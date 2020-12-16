using System;
using System.Collections.Generic;

namespace Carbon.Demo.WebApplication.Application.Dtos.Redis
{
    public class CompanyDto
    {
        public CompanyDto()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public List<LocationDto> Location { get; set; }
    }
}
