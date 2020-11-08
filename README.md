# SmashTracker

This application is currently under development.

This is a desktop application written to aid in the running of tournaments for Super Smash Bros. The final product will have support for all games in the series. This tool has features such as tracking rating automatically, using my implementation of Microsoft's TrueSkill algorithm, tournament creation and management tools using Challonge.com's API, and even match scheduling tools. 

The TrueSkill implementation was derived from [Jeff Moser's Repo](https://github.com/moserware/Skills), using his [Computing Your Skill article](http://www.moserware.com/2010/03/computing-your-skill.html) to help me understand how it works.

# Project Todos:
* Create player db page
* Configure ElectronCGI, and hook in app to backend
* Create page to update player rankings
* Create tournament hosting process
* Integrate with Challonge API
    * Optional: Integrate with SmashGG API, and allow users to pick between the two
* Optimize Electron
