using System;

[FlagsAttribute]
public enum Direction2D
{
	RIGHT = 1 << 0,
	LEFT = 1 << 1,
	ABOVE = 1 << 2,
	BELOW = 1 << 3,
	ALL = RIGHT | LEFT | ABOVE | BELOW
}
