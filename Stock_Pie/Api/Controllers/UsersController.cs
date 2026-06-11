using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stripe;

namespace Stock_Pie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        //private readonly IMediator _mediator;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UsersController(IUserContext userContext, IMapper mapper,IUserService userService)
        {
            //_mediator = mediator;
            _userContext = userContext;
            _mapper = mapper;
            _userService = userService;
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserRegisterDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            //var user = await _mediator.Send(new CreateUserCommand(dto));
            var outDto = _mapper.Map<UserDto>(user);
            return CreatedAtAction(nameof(GetById), new { id = outDto.Id }, outDto);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrent()
        {
            var userId = _userContext.UserId;
            if (userId == Guid.Empty) return Unauthorized();
            var user = await _userService.GetUserByIdAsync(userId);
            //var user = await _mediator.Send(new GetUserByIdQuery(userId));
            if (user == null) return NotFound();
            var outDto = _mapper.Map<UserDto>(user);
            return Ok(outDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            //var user = await _mediator.Send(new GetUserByIdQuery(id));
            if (user == null) return NotFound();
            var outDto = _mapper.Map<UserDto>(user);
            return Ok(outDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateById(Guid id ,UserUpdateDto dto)
        {
            var user = await _userService.UpdateUserAsync(id ,dto);
            //var user = await _mediator.Send(new GetUserByIdQuery(id));
            if (user == null) return NotFound();
            var outDto = _mapper.Map<UserDto>(user);
            return Ok(outDto);
        }

        [HttpGet("by-email")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email is required");
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null) return NotFound();
            var outDto = _mapper.Map<UserDto>(user);
            return Ok(outDto);
        }
    }
}