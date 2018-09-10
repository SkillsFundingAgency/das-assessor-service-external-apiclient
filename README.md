# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  External API Client (das-assessor-service-external-apiclient)

![Build Status](https://sfa-gov-uk.visualstudio.com/_apis/public/build/definitions/c39e0c0b-7aff-4606-b160-3566f3bbce23/???/badge)

### Developer Setup

##### Requirements

- Install [Visual Studio 2017](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - .NET desktop development
- Obtain External API Subscription Key and Base Address

##### Open the solution

- Open the solution
- Set SFA.DAS.AssessorService.ExternalApi.Client as the startup project
- Update the API Subscription Key and Base Address
- Running the solution will launch the desktop application

### SFA.DAS.AssessorService.ExternalApi.Examples

Example code should you wish to implement your own client :

- Program.cs - basic client
- ProgramCsv.cs - CSV enabled client

### SFA.DAS.AssessorService.ExternalApi.Core

Core functionality to interact with the External API.

### SFA.DAS.AssessorService.ExternalApi.Core.Tests

Unit and mocked Integration tests.

## License
Licensed under the [MIT license](https://github.com/SkillsFundingAgency/das-assessor-service-external-apiclient/blob/master/LICENSE)