Turtle is an cursor on the display which can be controlled to draw different shapes by inputting commands. 

Controls:
Light blue panel: Drawing area.
Commands listbox: Program to execute.
ComboBox: Allows to select a command or change selected command from command listbox.
Numeric Up Down: Change the value of selected command from command listbox.
Button "Add command": Adds selected command from ComboBox to the command listbox.
Button "Delete command": Delete selected command from the command listbox.
Button "Arrow Up": Select previous commmand from the command listbox.
Button "Arrow Down": Select next command from the command listbox.
Label "Stack/Instruction/Loop": This label is used to inform user about the:
-Stack (Nested Repeats commands. Maximum stack is 128).
-Instruction (command listbox) index.
-Loop repeats left. 
Button "Back": Previous command.
Button "Next": Next command.
Button "Start": Execute or Stop the program. 
ComboBox "Execution speed": Program execution speed in X/1000ms.

Commands: 
START = indicates the beggining of command, also used to reset the turtle and clear graphics.
FORWARD X = Moves turtle forward by X pixels. And draws a line if drawing is enabled
BACKWARD X = Moves turtle backwards by x pixels. And draws a line if drawing is enabled
LEFT X = Rotates turtle left by angle X.
RIGHT X = Rotates turlte right by angle X.
REPEAT X = Sets to Repeat command by X times. This has to be followed after by ENDREPEAT somewhere in the command list. 
ENDREPEAT = Indicates end of repeat command. 
DRAW = Starts drawing whenever turtles moves. Enabled by default.
STOPDRAW = Stops drawing whenever turtle moves.

