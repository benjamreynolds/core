﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Bit.Core.Models.Api;
using Bit.Core.Exceptions;
using Bit.Core.Services;

namespace Bit.Api.Controllers
{
    [Route("organizations/{orgId}/users")]
    [Authorize("Application")]
    public class OrganizationUsersController : Controller
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly IOrganizationService _organizationService;
        private readonly ISubvaultRepository _subvaultRepository;
        private readonly IUserService _userService;

        public OrganizationUsersController(
            IOrganizationRepository organizationRepository,
            IOrganizationUserRepository organizationUserRepository,
            IOrganizationService organizationService,
            ISubvaultRepository subvaultRepository,
            IUserService userService)
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _organizationService = organizationService;
            _subvaultRepository = subvaultRepository;
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<OrganizationUserDetailsResponseModel> Get(string orgId, string id)
        {
            var organizationUser = await _organizationUserRepository.GetDetailsByIdAsync(new Guid(id));
            if(organizationUser == null)
            {
                throw new NotFoundException();
            }

            return new OrganizationUserDetailsResponseModel(organizationUser.Item1, organizationUser.Item2);
        }

        [HttpGet("")]
        public async Task<ListResponseModel<OrganizationUserResponseModel>> Get(string orgId)
        {
            var organizationUsers = await _organizationUserRepository.GetManyDetailsByOrganizationAsync(new Guid(orgId));
            var responses = organizationUsers.Select(o => new OrganizationUserResponseModel(o));
            return new ListResponseModel<OrganizationUserResponseModel>(responses);
        }

        [HttpPost("invite")]
        public async Task Invite(string orgId, [FromBody]OrganizationUserInviteRequestModel model)
        {
            var user = await _userService.GetUserByPrincipalAsync(User);
            var result = await _organizationService.InviteUserAsync(new Guid(orgId), model.Email, model.Type,
                model.Subvaults?.Select(s => s.ToSubvaultUser()));
        }

        [HttpPut("{id}/accept")]
        [HttpPost("{id}/accept")]
        public async Task Accept(string orgId, string id, [FromBody]OrganizationUserAcceptRequestModel model)
        {
            var user = await _userService.GetUserByPrincipalAsync(User);
            var result = await _organizationService.AcceptUserAsync(new Guid(id), user, model.Token);
        }

        [HttpPut("{id}/confirm")]
        [HttpPost("{id}/confirm")]
        public async Task Confirm(string orgId, string id, [FromBody]OrganizationUserConfirmRequestModel model)
        {
            var result = await _organizationService.ConfirmUserAsync(new Guid(id), model.Key);
        }

        [HttpPut("{id}")]
        [HttpPost("{id}")]
        public async Task Put(string id, [FromBody]OrganizationUserUpdateRequestModel model)
        {
            var organizationUser = await _organizationUserRepository.GetByIdAsync(new Guid(id));
            if(organizationUser == null)
            {
                throw new NotFoundException();
            }

            await _organizationService.SaveUserAsync(model.ToOrganizationUser(organizationUser),
                model.Subvaults?.Select(s => s.ToSubvaultUser()));
        }

        [HttpDelete("{id}")]
        [HttpPost("{id}/delete")]
        public async Task Delete(string orgId, string id)
        {
            var organization = await _organizationRepository.GetByIdAsync(new Guid(id),
                _userService.GetProperUserId(User).Value);
            if(organization == null)
            {
                throw new NotFoundException();
            }

            await _organizationRepository.DeleteAsync(organization);
        }
    }
}
