using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class ReadingBetweentheLines : MonoBehaviour {

   /*
    * This code was written from 12 to 3. I am tired. I want to sleep, and this code prolly sucks.
    */

   public KMBombInfo Bomb;
   public KMAudio Audio;

   public TextMesh Display;

   public KMSelectable[] ColoredButtons;
   //public KMSelectable Submit;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   public GameObject[] EntireButtons;
   public TextMesh[] ButtonTexts;
   public GameObject[] ButtonTops;
   public Material[] ColorsMat;


   string[] WordList = { 
      "wyvern", "galaxy", "sulfer", "voodoo", "vacant",
      "ravage", "dharma", "uganda", "hijack", "hazard",
      "snazzy", "scheme", "coccyx", "gemini", "vacuum",
      "beyond", "empire", "twelve", "volume", "luxury",
      "tainte", "dlostt", "imehel", "lyeaaa", "gexish"
   };
   string[] ChosenWords = new string[4];

   string WordConglomerate = "";

   string[] Colors = { "R", "Y", "G", "B"};
   string ColorSolution = "";

   bool[] animatingFlag = new bool[4];

   int[] Splits = { 1, 2, 3, 0};
   bool[] Availability = new bool[4];
   int LastChosen = -1;
   int[] Progress = new int[4];

   List<int> RedInd = new List<int>() { };
   List<int> YelInd = new List<int>() { };
   List<int> GreInd = new List<int>() { };
   List<int> BluInd = new List<int>() { };

   int Pos;

   void Awake () {
      ModuleId = ModuleIdCounter++;
      
      foreach (KMSelectable Button in ColoredButtons) {
         Button.OnInteract += delegate () { ColorPress(Button); return false; };
      }
   }

   private IEnumerator keyAnimation (int index) {  //Stolen from Kavin
      animatingFlag[index] = true;
      ColoredButtons[index].AddInteractionPunch(0.125f);
      ButtonTops[index].GetComponent<MeshRenderer>().material = ColorsMat[4];
      Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
      for (int i = 0; i < 5; i++) {
         EntireButtons[index].transform.localPosition += new Vector3(0, -0.001F, 0);
         yield return new WaitForSeconds(0.005F);
      }
      for (int i = 0; i < 5; i++) {
         EntireButtons[index].transform.localPosition += new Vector3(0, +0.001F, 0);
         yield return new WaitForSeconds(0.005F);
      }
      ButtonTops[index].GetComponent<MeshRenderer>().material = ColorsMat[index];
      animatingFlag[index] = false;
   }


   void ColorPress (KMSelectable Button) {
      if (ModuleSolved) {
         return;
      }
      if (Button == ColoredButtons[0]) {
         RedInd.Add(Pos);
         StartCoroutine(keyAnimation(0));
         UpdateText("R");
      }
      if (Button == ColoredButtons[1]) {
         YelInd.Add(Pos);
         StartCoroutine(keyAnimation(1));
         UpdateText("Y");
      }
      if (Button == ColoredButtons[2]) {
         GreInd.Add(Pos);
         StartCoroutine(keyAnimation(2));
         UpdateText("G");
      }
      if (Button == ColoredButtons[3]) {
         BluInd.Add(Pos);
         StartCoroutine(keyAnimation(3));
         UpdateText("B");
      }
      Pos++;
      if (Pos == 24) {
         Check();
      }
      //Debug.Log()
   }

   void Check () {
      string word = "";
      bool willSolve = true;
      for (int i = 0; i < RedInd.Count(); i++) {
         word += WordConglomerate[RedInd[i]].ToString(); //Checks if each color list is a word
      }
      //Debug.Log(word);
      if (!CheckHelper(word)) {
         willSolve = false;
      }
      word = "";
      for (int i = 0; i < RedInd.Count(); i++) {
         word += WordConglomerate[RedInd[i]].ToString();
      }
      if (!CheckHelper(word)) {
         willSolve = false;
      }
      word = "";
      for (int i = 0; i < RedInd.Count(); i++) {
         word += WordConglomerate[RedInd[i]].ToString();
      }
      if (!CheckHelper(word)) {
         willSolve = false;
      }
      word = "";
      for (int i = 0; i < RedInd.Count(); i++) {
         word += WordConglomerate[RedInd[i]].ToString();
      }
      if (!CheckHelper(word)) {
         willSolve = false;
      }

      if (willSolve) {
         GetComponent<KMBombModule>().HandlePass();
         ModuleSolved = true;
      }
      else {
         GetComponent<KMBombModule>().HandleStrike();
         FullReset();
      }
   }

   bool CheckHelper (string s) {
      return WordList.Contains(s.ToLower());
   }

   void FullReset () {
      RedInd.Clear();
      BluInd.Clear();
      YelInd.Clear();
      GreInd.Clear();
      ChosenWords = WordList.Shuffle().Take(4).ToArray();
      Splits[0] = 1;
      Splits[1] = 2;
      Splits[2] = 3;
      Splits[3] = Rnd.Range(1, 4);
      for (int i = 0; i < 4; i++) {
         Progress[i] = 0;
         Debug.LogFormat("[Reading Between the Lines #{0}] the word \"{2}\" is split into {1} groups of {3}.", ModuleId, 6 / Splits[i], ChosenWords[i], Splits[i]);
         Availability[i] = true;
      }
      WordConglomerate = "";
      Pos = 0;
      Splits.Shuffle();
      LastChosen = -1;
      ColorSolution = "";
      SplitWords();
   }

   void UpdateText (string c) {  //Richtext moment
      //Display.richText = true;
      switch (c) {
         case "R":
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 1, "<color=#ff0000ff>");
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 2, "</color>");
            //Debug.Log(Display.text);
            break;
         case "Y":
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 1, "<color=#ffff00ff>");
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 2, "</color>");
            break;
         case "G":
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 1, "<color=#00ff00ff>");
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 2, "</color>");
            break;
         case "B":
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 1, "<color=#0000ffff>");
            Display.text = Display.text.Insert(Display.text.LastIndexOf('>') + 2, "</color>");
            break;
         default:
            
            break;
      }
   }

   void Start () {
      ChosenWords = WordList.Shuffle().Take(4).ToArray();
      Splits[3] = Rnd.Range(1, 4);
      Splits.Shuffle();
      for (int i = 0; i < 4; i++) {
         Availability[i] = true;
         Debug.LogFormat("[Reading Between the Lines #{0}] the word \"{2}\" is split into {1} groups of {3}.", ModuleId, 6 / Splits[i], ChosenWords[i], Splits[i]);
      }
      SplitWords();
   }

   void SplitWords () {
      
      while (WordConglomerate.Length < 24) { //6 * 4 = 24.
         int rand = ChosenSplitChoice();
         for (int i = 0; i < Splits[rand]; i++) {
            WordConglomerate += ChosenWords[rand][Progress[rand] + i].ToString();
            ColorSolution += Colors[rand];
         }
         Progress[rand] += Splits[rand];
         if (Progress[rand] == 6) {
            Availability[rand] = false;
         }
      }

      Display.text = WordConglomerate;
      Debug.LogFormat("[Reading Between the Lines #{0}] The shown mess is {1}.", ModuleId, WordConglomerate);
      Debug.LogFormat("[Reading Between the Lines #{0}] One possible solution is {1}.", ModuleId, ColorSolution);
   }

   int CountAvailableWords () {
      int count = 0;
      for (int i = 0; i < 4; i++) {
         if (Availability[i]) {
            count++;
         }
      }
      return count;
   }

   int ChosenSplitChoice () {
      int l = 0;
      Reroll:
      if (CountAvailableWords() <= 1) {   //If there is one group left, make sure there is exactly enough space for it to finish out, if not we reset
         for (int i = 0; i < 4; i++) {
            if (Availability[i]) {
               if (24 - WordConglomerate.Length == Splits[i]) {
                  return i;
               }
               else {
                  ResetSplitting();
                  goto Reroll;
               }
            }
         }
         return -1;
      }
      do {
         l = Rnd.Range(0, 4);
      } while (LastChosen == l || !Availability[l]);  //Makes sure that the last word is not the same as the current one.
      LastChosen = l;
      return l;
      return 8008135; //boobies
   }

   void ResetSplitting () {
      ColorSolution = "";
      for (int i = 0; i < 4; i++) {
         Progress[i] = 0;
         WordConglomerate = "";
         Availability[i] = true;
         LastChosen = -1;
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} RYGB to press that button.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {   //Shit.
      Command = Command.Trim().ToUpper();
      yield return null;
      for (int i = 0; i < Command.Length; i++) {
         switch (Command[i]) {
            case 'R':
               ColoredButtons[0].OnInteract();
               break;
            case 'Y':
               ColoredButtons[1].OnInteract();
               break;
            case 'G':
               ColoredButtons[2].OnInteract();
               break;
            case 'B':
               ColoredButtons[3].OnInteract();
               break;
            default:
               yield return "sendtochaterror I don't understand!";
            break;
         }
         yield return new WaitForSeconds(.1f);
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      while (!ModuleSolved) {
         switch (ColorSolution[Pos]) {
            case 'R':
               ColoredButtons[0].OnInteract();
               break;
            case 'Y':
               ColoredButtons[1].OnInteract();
               break;
            case 'G':
               ColoredButtons[2].OnInteract();
               break;
            case 'B':
               ColoredButtons[3].OnInteract();
               break;
         }
         yield return new WaitForSeconds(.1f);
      }
   }
}
