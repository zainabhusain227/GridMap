# GridMap
Unity Application to allow navigation of csv file data using keyboard commands. Table contents is displayed through text-to-speech and spatial audio

# Code structure
All scripts can be found under GridMap\Assets\Scripts

The main functions of each script are described below:

CSVReaderFinal:Loads in the CSV file, sets audio sources for ambient audio

GridManager: Created the grid for the game, keeps track of and updates players location

GridNavigator: This is where the various keyboard inputs are defined. Movement related features such as move and jump are also defined here

SoundManager: This script updates the location each ambient sound is coming from as the player moves around the grid
