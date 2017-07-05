# nadaconfig
NadaConfig (An easy and simple centralized configuration system)

## Project Description

NadaConfig is a centralized configuration system that will let client applications retrieve their configuration data from a unique store on a server.
Ideal in a distributed environment, it simplifies the deployment of configuration changes. NadaConfig is non-intrusive (can be as easy a adding one file to a client project) with a minuscule footprint.
Goals

* Lightweight
* Simple to use
* Minimum configuration; most cases one parameter.(Otherwise it would defeat the purpose)
* Facilitate the deployment of configuration changes to multiple clients.(Centralization)
* Clients discover servers automatically so the servers can be changed without any impact.
## Use Case Scenario

**The Problematic**  
You have a distributed client-server application consisting of several environments; ex. Production, Test, Training, Development, DisasterRecovery etc.
Every time that you want a client to target a different environment, you need to change a multitude of configuration files with a lot of parameters. If you want to change one client parameter; you need to deploy it on every client.  

**The Solution**  
You install a NadaConfig server on each environment (You could also use only one that will serve all the environments). The client portion of NadaConfig only needs to know which environment to target; this is the only parameter needed.
The client will search for a server that manages that environment and when it is found, it will request its configuration parameters from it. The beauty of the discovery process is that the client is only aware of the environment to search for; you can change the servers at will and the client will find the right one.
## Usage

See the documentation --> [Simple Usage](https://github.com/KurdyMalloy/nadaconfig/wiki/Simple-Usage)
## How it works

See the documentation --> [Documentation](https://github.com/KurdyMalloy/nadaconfig/wiki/Documentation)
