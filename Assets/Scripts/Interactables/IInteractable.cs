using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Interactable
{

    public interface IInteractable
    {
        string HighlightText { get; } //consider having this just return a string, currently no reason to supply the interaction text + adds depenency to any domain with an Iinteractable
        void Interact(GameObject interacter);
        bool InteractionEnabled { get => true; }
    }
}