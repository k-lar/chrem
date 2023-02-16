# chrem (~~csharp reminders~~)

This program is a part of my `*rem` series of programs that I create to get used to working with a
new programming language. This one is made with C#, and built with [mono](https://www.mono-project.com/).

## Features

- Adding / removing entries
- Automatic renumbering

## Installation

**Linux:**
```console
# Clone git repo and go into directory
git clone https://gitlab.com/k_lar/chrem; cd chrem

# Install
mcs chrem.cs
```

[Download windows binary](https://gitlab.com/k_lar/chrem/uploads/1f0570beeaf0e98f306afd562011cb4f/chrem.zip)


## Usage (Linux)

```console
# Adding entries:
mono chrem.exe -a "Very important note"

# Removing entries:
# This one removes entry [1]
mono chrem.exe -r 1

# This removes entries [7], [3] and [2]
mono chrem.exe -r 7 -r 3 -r 2

# Display entries:
mono chrem.exe --show

# You can chain commands like so:
mono chrem.exe -r 3 -a "Meeting at 12:00" --show

# For additional commands
mono chrem.exe -h
```

Example:

```console
user@pc:~$ chrem --show
[1] - Release first C# program
[2] - Check on dad
[3] - Cool website: https://www.lofi.cafe/
[4] - Learn clojure
[5] - Workout
[6] - Release chrem 1.0
```


## Feedback

Any and all feedback is appreciated. Please tell me how to OOP better, I have no idea.

