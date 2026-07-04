using System;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

Console.WriteLine("Hello World");

string pathToInputMidiFile = Console.ReadLine();

ReadingSettings readingSettings = new ReadingSettings
{
    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits
};

MidiFile midiFile = MidiFile.Read(pathToInputMidiFile, readingSettings);

int trackNumber = 0;
List<Classes.Note> notes = new List<Classes.Note>();

foreach (TrackChunk trackChunk in midiFile.GetTrackChunks())
{
    trackNumber++;

    foreach (Note note in trackChunk.GetNotes())
    {
        notes.Add(new Classes.Note
        {
            TrackNumber = trackNumber,
            NoteName = note.NoteName.ToString(),
            Time = note.Time,
            Length = note.Length
        });
    }
}