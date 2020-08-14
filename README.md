# Hackweek 2020 Explore ML Agents
This hackweek we decided to checkout what ML Agents can do, how it works, can people without much knowledge in AI learn to use it quickly as well?

## Setup
### Unity version
This project uses Unity 2019.4.1f1 LTS

### Templates
There are two kinds of templates, one for environments and one for your own experimentation's. Place these in the root of the `Assets` folder

#### Environment
To setup a new environment simple duplicate the `Templates/ENVIRONMENT_TEMPLATE` folder and rename both the folder and the `Templates/TEMPLATE_SCENE` scene file plus folder that live inside it to the name of the environment

#### User
In order to ensure that we don't accidentally mess with each others code or cause conflicts you should duplicate a `Templates/USER_TEMPLATE` and rename both the folder and the `Templates/TEMPLATE_SCENE` scene file plus folder that live inside it to your own name

## Custom Packages
Custom packages are both allowed and encouraged as long as they are freely available. They can come from Github, OpenUPM, The Asset Store or anywhere else you can think off as long as they're free to access.

### Change Camera Aim On Start
When you open up the project you will see a button "Start at scene position" next to the play button, this is done through [this package](https://github.com/MileyHollenberg/ChangeCameraAimOnStart) and will ensure that when you enter play mode the camera position in the scene view gets copied over to the game view. If you want to turn this off just press the button and it will switch back to the default Unity behaviour.

## Shared resources
In case you have certain resources that aren't specific to an environment or your own experimentation's then please put those in the `Shared` folder so we can all make use of them