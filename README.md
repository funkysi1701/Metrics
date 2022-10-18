# Metrics

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=funkysi1701_Metrics&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=funkysi1701_Metrics)

[![codecov](https://codecov.io/gh/funkysi1701/Metrics/branch/main/graph/badge.svg?token=HPYqPbjhPQ)](https://codecov.io/gh/funkysi1701/Metrics)

This is an application that logs various metrics about a user. 

Intended to be both useful and an exercise in architecture and building something.

v1 is currently running at [metrics.funkysi1701.com](https://metrics.funkysi1701.com/) this repo will be a rebuild/rearchitecture with the following features.

- Backend DB to be as cheap as possible, currently using CosmosDB but suspect something like MongoDB may be cheaper
- Ability to track mutiple users on multiple platforms
- Twitter, Github, dev.To, rss feeds, energy?? all supported
- Blazor webassembley front end running on Azure Static Web Apps (or do I want to try CloudFlare pages?)
- Azure Function backend
- If possible import existing data into new system
- Pulumi to deploy and setup infrastructure
- Signup/login system?

---
