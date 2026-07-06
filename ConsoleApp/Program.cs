using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

Console.WriteLine("Enter path to configuration file (.json):");
string pathToConfigurationFile = Console.ReadLine();
Classes.Configuration configuration = null;

using (StreamReader streamReader = new StreamReader(pathToConfigurationFile))
{
    string json = streamReader.ReadToEnd();

    configuration = JsonSerializer.Deserialize<Classes.Configuration>(json);
}

ReadingSettings readingSettings = new ReadingSettings
{
    InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.SnapToLimits
};
MidiFile midiFile = MidiFile.Read(configuration.pathToInputMidiFile, readingSettings);
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

Classes.Lyrics lyrics = null;

using (StreamReader streamReader = new StreamReader(configuration.pathToInputTimedLyricsFile))
{
    string json = streamReader.ReadToEnd();

    lyrics = JsonSerializer.Deserialize<Classes.Lyrics>(json);
}
