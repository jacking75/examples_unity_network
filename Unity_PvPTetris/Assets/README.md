# VGS-Tetris

Simple 2D Tetris Game in Unity

v0.0.2-r12

Added Online High Scores


Added ShowOnlineScores()

Added DownloadHighscoresFromDatabase()

Added UploadNewHighScore()

Uses dreamlo.com to store scores

<img src="https://i.gyazo.com/6ace1a67a6d98d4acfb9c629c055d195.gif">

<img src="https://i.gyazo.com/da38dc321ca153ef8e28c069c84bcb9c.png">


v0.0.1-r11

Added Pause Menu (Press Esc)

Fixed bug where screen wasn't flashing on Level 1 for quadruple lines


v0.0.1-r10

Added 'High Scores' Panel

Added ShowHighScores();

Added Score Multiplier with Level Up

Added Score Multiplier for double/triple/quadruple Lines

Now using standardized colors for Tetrominos

Screen now flashes on quadruple lines

<img src="https://i.gyazo.com/e4de58b250221577e20fd18d50f6d4d2.gif">


v0.0.1-r9

Now loads HighScore for set PlayerName

Added LoadHighScore();

<img src="https://i.gyazo.com/9b811da6312cbdf0cfd237f856d1c345.png">


v0.0.1-r8

PlayerName is now properly displayed in Settings Panel

Music Toggle is now saved to file.

Settings are now loaded from file when the game starts

Also when the settings panel is opened.

Player Name is now shown above 'High Score' on Game Scene.

Moved 'TitleText' slightly left on Game Scene.

<img src="https://i.gyazo.com/de0e72fae24dab963d11bb7362691970.png">


v0.0.1-r7

Added Background to MainMenu

Added Settings Menu

Added Player Name

Added Music

PlayerName and HighScore now saved to file.

Added Levels

FallSpeed now increases on Level Up

Added SaveSettings();

Added SavePlayer();

Added SaveMusic();

Added LoadPlayer();

Added WriteHighScore();

Music Source: http://opsound.org/artist/macroform/

<img src="https://i.gyazo.com/52b1a79ed9a18ed573879e06e4adf85b.jpg">


v0.0.1-r6

Moved block spawner up a few lines

Added High Score (not persistent yet)

Added GameManager Object

Added UpdateHighScore();

Added RefreshHighScore();

Added NewGame();

GameOverPanel now properly destroys itself

<img src="https://i.gyazo.com/7f1e18d7354d1344ef77d1538efbd36f.png">


v0.0.1-r5

Added two more colors for textures

Each Object has its own unique color now

Increased size of Score/Lines text in Game Scene

Added GameOverPanel PreFab

Added Restart button to GameOverPanel PreFab

Added Quit button to GameOverPanel PreFab

Added GameOverFn();

<img src="https://i.gyazo.com/4e0584ba5dbb3dfb9113bcd7f1ccec55.png">


v0.0.1-r4

Added different color textures

Updated GitHub text to reflect Username/Repo

Added 'Lines' text to Score Label

Increment 'Score' Value by 12 on positive isRowFull() result

Incrememnt 'Line' Value by 1 on positive isRowFull() result

Added TitleText to left side of Game Scene

<img src="https://i.gyazo.com/cd5c2f6f32cce4f822b62c54da4902b6.png">


v0.0.1-r3

Darkened Main Menu Button Text

Added Main Menu Button Highlight

Created Canvas on Game Scene

Moved FrameRateCounter to Game Scene Canvas

Added 'Score' Label to Game Scene

Increment 'Score' Label by 10 on positive isRowFull() result

Reorganized Assets into Folders

<img src="https://i.gyazo.com/22d66f04a77c903c789e0f445405e8e5.png">


v0.0.1-r2

Added Main Menu

Added Play Button

Added Exit Button

Removed unused Standard Assets


v0.0.1

Initial Build

Follows Tutorial from: https://noobtuts.com/unity/2d-tetris-game

<img src="https://i.gyazo.com/1418f5e65f51eac67514312ca1557d0f.png">