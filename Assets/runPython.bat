@echo off
REM Name of the conda environment
set CONDA_ENV=conda_env

REM Initialize Conda (adjust path if necessary)
call "C:\Users\kopro\anaconda3\condabin\conda.bat" shell.cmd.exe activate

REM Activate the conda environment
call conda activate %CONDA_ENV%

REM Check if conda environment activation was successful
if %ERRORLEVEL% NEQ 0 (
    echo Failed to activate conda environment %CONDA_ENV%
    pause
    exit /b %ERRORLEVEL%
)

REM Get the prompt parameter
set PROMPT=%1
set NAME=%2


REM Run the Python script
python "C:\Users\kopro\Documents\_Projekte\Unity\AutomaticSoundDesign\Assets\TTA\gen_wav.py" --prompt ""%PROMPT%"" --ddim_steps 50 --duration 10 --scale 20 --n_samples 1 --save_name "C:\Users\kopro\Documents\_Projekte\Unity\AutomaticSoundDesign\Assets\Resources\%NAME%"

REM Check if python script execution was successful
if %ERRORLEVEL% NEQ 0 (
    echo Python script failed
    pause
    exit /b %ERRORLEVEL%
)

echo Script executed successfully