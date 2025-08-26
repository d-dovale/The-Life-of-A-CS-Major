// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Michael O'Connell, then edited by Benjamin Cohen, Eric Bejleri, and Logan Kemper

using System.Collections.Generic;
using UnityEngine;

namespace DigitalWorlds.Dialogue
{
    /// <summary>
    /// Sends dialogue to the DialogueManager via trigger collision or button press.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        [Tooltip("Drag in the DialogueManager.")]
        [SerializeField] private DialogueManager dialogueManager;

        [Tooltip("Drag in the text file for this dialogue.")]
        [SerializeField] private TextAsset textFile;

        [Tooltip("Enter the tag name that should register collisions.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("How long in seconds before a new line of dialogue can be skipped.")]
        [SerializeField] private float waitTime = 0.5f;

        [Tooltip("If true, this dialogue can only be triggered one time.")]
        public bool singleUse = false;

        [Tooltip("Choose whether dialogue will be triggered by a key press or a trigger collision.")]
        [SerializeField] private TriggerType triggerType;

        [HideInInspector] public bool hasBeenUsed = false;

        private Queue<string> dialogue = new();
        private bool inArea = false;
        private float nextTime = 0f;

        public enum TriggerType
        {
            TriggerCollision,
            KeyPress
        }

        private void Start()
        {
            if (dialogueManager == null)
            {
                dialogueManager = FindAnyObjectByType<DialogueManager>();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && !hasBeenUsed && inArea)
            {
                if (!dialogueManager.IsInDialogue && triggerType == TriggerType.KeyPress)
                {
                    TriggerDialogue();
                }
                else if (nextTime < Time.timeSinceLevelLoad)
                {
                    nextTime = Time.timeSinceLevelLoad + waitTime;
                    dialogueManager.AdvanceDialogue();
                }
            }
        }

        public void TriggerDialogue()
        {
            dialogueManager.CurrentTrigger = this;
            ReadTextFile();
            dialogueManager.StartDialogue(dialogue);
        }

        private void ReadTextFile()
        {
            dialogue.Clear();
            string txt = textFile.text;
            string[] lines = txt.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                int tagEnd;
                string currentLine = line;

                // Process any tags at the beginning of the line
                while (currentLine.StartsWith("[") && (tagEnd = currentLine.IndexOf(']')) != -1)
                {
                    string tag = currentLine.Substring(0, tagEnd + 1);
                    dialogue.Enqueue(tag);
                    currentLine = currentLine.Substring(tagEnd + 1);
                }

                // Add the remaining text if any
                if (!string.IsNullOrEmpty(currentLine))
                {
                    dialogue.Enqueue(currentLine);
                }
            }

            dialogue.Enqueue("EndQueue");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(tagName) && !hasBeenUsed)
            {
                if (triggerType == TriggerType.TriggerCollision)
                {
                    TriggerDialogue();
                }

                inArea = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(tagName))
            {
                dialogueManager.EndDialogue();

                inArea = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(tagName) && !hasBeenUsed)
            {
                if (triggerType == TriggerType.TriggerCollision)
                {
                    TriggerDialogue();
                }

                inArea = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(tagName))
            {
                dialogueManager.EndDialogue();

                inArea = false;
            }
        }

        private void OnValidate()
        {
            // Clamp waitTime to 0 in the inspector
            waitTime = Mathf.Max(0, waitTime);
        }
    }
}