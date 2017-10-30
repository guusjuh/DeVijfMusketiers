﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command<T> {
    public virtual bool Execute(T controller) { return false; }
    public virtual bool Undo(T controller) { return false; }
}
