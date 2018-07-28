﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell2Forward : Spell {

	// Use this for initialization

	public Spell2Forward () {
		spellName = "Forward";
		type = 0;
	}

	public override void UseSpell(){
		InGameManager.Instance.spellText.text = spellName;
		InGameManager.Instance.UseSpell2Forward();
	}
}
