#!/bin/bash


# allow the library to build
mv main.HIDE main.cs
mcs *.cs -r:System.Drawing.dll
mv main.cs main.HIDE

# remove the .exe
if test -f "main.exe"; then
    rm main.exe
fi