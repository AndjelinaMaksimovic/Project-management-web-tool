<h1>Contents</h1>

[README in Serbian language](README.srb.md)

- [Development Version](#development-version)
    - [Installations](#installations)
    - [Running](#running)
- [Production Version](#production-version)
    - [1. Running in an active terminal](#1-running-in-an-active-terminal)
    - [2. Running in the background (command)](#2-running-in-the-background-command)
      - [Stopping the tool running in the background](#stopping-the-tool-running-in-the-background)
- [Credentials for all user types in the system](#credentials)

**The application is available at: http://softeng.pmf.kg.ac.rs:10123/**

# Development Version

### Installations
To run the web application, the following installations are required:<br><br>

**Node.js and npm** - These can be downloaded from the [official Node.js website](https://nodejs.org/en). To verify if npm, the package manager for Node.js, is installed, type the `npm` command in the terminal. If you get an error that the command is not recognized, install npm using the command `npm install` in the terminal.

**Angular CLI** - Install using the npm command `npm install -g @angular/cli`.

**.NET Core SDK** - Download and install it from [here](https://dotnet.microsoft.com/en-us/download).

### Running

Clone the application repository from GitLab using the following command in the terminal: 
`git clone https://gitlab.pmf.kg.ac.rs/si2024/codedberries.git`

**Running the Backend**
The folder where the repository is cloned will contain a folder named *Codedberries*. To run the backend of the application, navigate to the folder containing Program.cs in the terminal using this command: 
`cd Codedberries/Src/BE/Codedberries/Codedberries`

To start the development version, run:
`git checkout dev`

From this location, start the backend using the command:
`dotnet run`

The build process that occurs when this command is executed may take a little while. When it's done, the first info response should indicate the port on which the backend is running, as shown in the image below.

![terminal output after the *dotnet run* command](images/dotnetRun.JPG)

To view the backend-developed endpoints, this command also launches *Swagger* for API testing. It can be accessed by appending */swagger* to the address returned by the `dotnet run` command. For example, it would be: *http://localhost:5285/swagger*.

**Running the Frontend**
Open another terminal and navigate to the folder where the repository was cloned.
You can do this easily by opening the folder in which the repository resides via File Explorer. Once in it, right-click and select the option *Open in terminal*. This will open a terminal window with the correct working directory.
Navigate to the angular-app folder using the command:
`cd Codedberries/Src/FE/angular-app`

To start the development version, run:
`git checkout dev`

To run the frontend, type the command:
`ng serve`

As a response to this command, the port on which the application is running will be shown (default Angular application port is 4200). The response should look like this:
    
![terminal output after the *ng serve* command](images/ngServe.JPG)

If the backend was successfully started previously, opening the frontend address should result in the tool being successfully launched.

<!---------------------------------------------------------------------------------------------------------------------->

# Production Version

To run the production version from the terminal of the web server *softeng.pmf.kg.ac.rs*, navigate to the folder *codedberries*. Verify this by running the command `pwd` in the terminal, which should return:
![pwd](images/pwd.JPG)

There are two useful ways to start the production version:

### 1. Running in an active terminal

When the tool is started in this way, it will remain active during the terminal session.

**Backend** - Navigate to the following folder: `cd BE/out` and start the backend using the command: `dotnet Codedberries.dll --urls=http://0.0.0.0:10122`.

**Frontend** - Open another terminal and navigate to the *codedberries* folder as mentioned above. Navigate to the folder: `cd FE/dist` and start the frontend using the command: `python3 error_script.py`.

Once both the frontend and backend are running, open the following address: 

### 2. Running in the background (command)
When the tool is started in this way, it runs in the background and remains active even when the terminal is closed.

**Backend** - Navigate to the following folder: `cd BE/out` and start the backend using the command:
`nohup dotnet Codedberries.dll --urls=http://0.0.0.0:10122 > /dev/null 2>&1 &`.

The response to this command is the PID of the backend process:
![PID of the running backend](images/detachedBE.JPG)

**Frontend** - From the same terminal, navigate to the *codedberries* folder as mentioned earlier. Navigate to the folder: `cd FE/dist` and start the frontend using the command:
`nohup python3 error_script.py > /dev/null 2>&1 &`.

Similar to the backend, the response to the command will be the PID of the frontend process.

#### Stopping the tool running in the background
If the PIDs of the frontend and backend processes were not saved, they can be retrieved using the following commands:

**Backend** - `ps aux | grep Codedberries.dll`. The output will show:
![PID of the running backend](images/killBE.JPG)
Stop the backend with `kill <PID>`, in this case: `kill 997636`.

**Frontend** - `ps aux | grep "python3 -m http.server 10123"`. The output will show:
![PID of the running frontend](images/killFe.JPG)
Stop the frontend with `kill <PID>`, in this case: `kill 995490`.

# Credentials for all user types in the system

**Super user** - username: petar.simic@gmail.com  password: password1  
**Project owner** - username: zoran.gajic@gmail.com  password: password3  
**Project manager** - username: lazar.milojevic@gmail.com  password: password3  
**Employee** - username: ana.dacic@gmail.com  password: password3  
**Viewer** - username: mina.markovic@gmail.com  password: password3

For both running methods, the tool can be accessed at: *http://softeng.pmf.kg.ac.rs:10123*.
