using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.Extensions.Logging;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;


        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {

            try
            {

                var countries = await _unitOfWork.Countries.GetAll();
                var results = _mapper.Map<IList<CountryDto>>(countries);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(GetCountries)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later");

            }

        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCountry(int id)
        {

            try
            {

                var country = await _unitOfWork.Countries.Get(e => e.Id == id, new List<string>{ "Hotels" });
                var results = _mapper.Map<CountryDto>(country);
                return Ok(results);

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Something went wrong in {nameof(GetCountry)}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later");

            }

        }


    }
}
