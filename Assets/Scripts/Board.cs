using UnityEngine ;
using UnityEngine.Events ;

public class Board : MonoBehaviour {
   [Header ("Input Settings : ")]
   [SerializeField] private LayerMask boxesLayerMask ;
   [SerializeField] private float touchRadius ;

   [Header ("Mark Sprites : ")]
   [SerializeField] private Sprite spriteX ;
   [SerializeField] private Sprite spriteO ;

   [Header ("Mark Colors : ")]
   [SerializeField] private Color colorX ;
   [SerializeField] private Color colorO ;

   public UnityAction<Mark,Color> OnWinAction ;

   public Mark[] marks ;

   private Camera cam ;

   private Mark currentMark ;

   private bool canPlay ;

   private LineRenderer lineRenderer ;

   private int marksCount = 0 ;

   private void Start () {
      cam = Camera.main ;
      lineRenderer = GetComponent<LineRenderer> () ;
      lineRenderer.enabled = false ;

      currentMark = Mark.X ;

      marks = new Mark[9] ;

      canPlay = true ;
   }

   private void Update () {
      if (canPlay && Input.GetMouseButtonUp (0)) {
         Vector2 touchPosition = cam.ScreenToWorldPoint (Input.mousePosition) ;

         Collider2D hit = Physics2D.OverlapCircle (touchPosition, touchRadius, boxesLayerMask) ;

         if (hit)//box is touched.
            HitBox (hit.GetComponent <Box> ()) ;
      }
   }

   private void HitBox (Box box) {
      if (!box.isMarked) {
         marks [ box.index ] = currentMark ;

         box.SetAsMarked (GetSprite (), currentMark, GetColor ()) ;
         marksCount++ ;

         //check if anybody wins:
         bool won = CheckIfWin () ;
         if (won) {
            if (OnWinAction != null)
               OnWinAction.Invoke (currentMark, GetColor ()) ;

            Debug.Log (currentMark.ToString () + " Wins.") ;

            canPlay = false ;
            return ;
         }

         if (marksCount == 9) {
            if (OnWinAction != null)
               OnWinAction.Invoke (Mark.None, Color.white) ;

            Debug.Log ("Nobody Wins.") ;

            canPlay = false ;
            return ;
         }

         SwitchPlayer () ;
      }
   }

   private bool CheckIfWin () {
      return
      AreBoxesMatched (0, 1, 2) || AreBoxesMatched (3, 4, 5) || AreBoxesMatched (6, 7, 8) ||
      AreBoxesMatched (0, 3, 6) || AreBoxesMatched (1, 4, 7) || AreBoxesMatched (2, 5, 8) ||
      AreBoxesMatched (0, 4, 8) || AreBoxesMatched (2, 4, 6) ;

   }

   private bool AreBoxesMatched (int i, int j, int k) {
      Mark m = currentMark ;
      bool matched = (marks [ i ] == m && marks [ j ] == m && marks [ k ] == m) ;

      if (matched)
         DrawLine (i, k) ;

      return matched ;
   }

   private void DrawLine (int i, int k) {
      lineRenderer.SetPosition (0, transform.GetChild (i).position) ;
      lineRenderer.SetPosition (1, transform.GetChild (k).position) ;
      Color color = GetColor () ;
      color.a = .3f ;
      lineRenderer.startColor = color ;
      lineRenderer.endColor = color ;

      lineRenderer.enabled = true ;
   }

   private void SwitchPlayer () {
      currentMark = (currentMark == Mark.X) ? Mark.O : Mark.X ;
   }

   private Color GetColor () {
      return (currentMark == Mark.X) ? colorX : colorO ;
   }

   private Sprite GetSprite () {
      return (currentMark == Mark.X) ? spriteX : spriteO ;
   }
}
