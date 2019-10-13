using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ball : MonoBehaviour
{
    //public
    //init ball velocity
    public float velocity = 5;
    public float paddleForceThreshold=4;
    public float speedUpTime = 5.0f;

    //private 
    private Vector3 _direction;
    [SerializeField] private Player _player;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _particle;
    [SerializeField] private AudioClip _clip;

    private Rigidbody2D _rb;
    private SpriteRenderer _spriterenderer;
    private AudioSource _audioSource;
    private bool _ifHitPaddle = true;
    private int _greyCount = 0;
    private bool _redBuffActivated = false;
    private bool isGameOver = false;
    private bool gameStart = false;
    // Start is called before the first frame update
    void Start()
    {
        //initialize a random direction for the ball
        float rand = Random.Range(-1.0f, 1.0f);
        int Xsign =(int) Mathf.Sign(rand);
        _direction = new Vector3(Xsign, 1, 0);
        _direction.Normalize();

        //cache refrence to rigidbody2d
        _rb = GetComponent<Rigidbody2D>();
        //cache reference to spiritRenderer
        _spriterenderer = GetComponent<SpriteRenderer>();
        //cache reference to audio souce
        _audioSource = GetComponent<AudioSource>();
        //hide mouse
        Cursor.visible = false;
        //nullity check
        if (!_clip) return;
        

    }

    // Update is called once per frame
    void Update()
    {
        //start the game by left clicking
        if (Input.GetMouseButtonDown(0))
            gameStart = true;

        //move the ball and paddle before the game start
        if (!gameStart)
        {
            transform.position = new Vector3(_player.transform.position.x, transform.position.y, transform.position.z);
            //disable the rigidbody to avoid losing the force to push the ball up
            _rb.Sleep();
        }
        else
        {
            //wake up the ridgid body
            _rb.WakeUp();
            if (_player)
            {
                //adding the position to the ball
                transform.position += _direction * velocity * Time.deltaTime;
            }
        }

       /// velocity = Mathf.Max(5, velocity -= 0.05f);
    }

    //when colliding with the box
    void OnCollisionEnter2D(Collision2D collision)
    {
        //cache collider reference
        Player _player = collision.gameObject.GetComponent<Player>();
        BoxEdge _box = collision.gameObject.GetComponent<BoxEdge>();
        Vector2 normal = collision.contacts[0].normal;

        
        
        //check if hit the edge collider 
        if (_box)
        {
            //check if it touches the bottom
            if (normal == Vector2.up)
            {
                //if it does, gameover
                isGameOver = true;
            }
        }
        //it it hits the paddle
        else if (_player)
        {
            _ifHitPaddle = true;
            //get key movement direction
            float direction = Input.GetAxis("Horizontal");
            //get mouse movement direction
            float mouseDir = Input.GetAxis("Mouse X");
            //if any key bind to "Horizontal" is pressed
            if (direction != 0)
            {
                //add the velocity to the ball
                velocity += (Mathf.Abs (direction)* paddleForceThreshold);
            }
            //if using mouse
            else if (mouseDir != 0)
            {
                //add the velocity to the ball
                velocity += (Mathf.Abs(mouseDir)* paddleForceThreshold);
            }
            
         
            //if its not vector2.up, then gameover
           if(normal != Vector2.up)
            {
                //isGameOver = true;
            }
            //play the sound
            _player.GetComponent<AudioSource>().Play();
        }
        //if it hits a wall
        else if (collision.gameObject.tag == "wall")
        {
            //Debug.Log("hit a wall");
        }
        //if it hits a grey block,add score and destroy
        else if(collision.gameObject.tag=="grey")
        {
            //calculate chaining score
            if (_ifHitPaddle == false)
            {
                //if red buff is activated, get additional 5 points
                if(_redBuffActivated)
                    GameManager.score += 7;
                else
                    GameManager.score += 2;

            }
            //normal score
            else
            {
                if (_redBuffActivated)
                    GameManager.score += 6;
                else
                    GameManager.score ++;
            }
            //count the grey ones hitted
            _greyCount++;
            _ifHitPaddle = false;
            //use particle system
            Instantiate(_particle, collision.gameObject.transform.position,Quaternion.identity);
            //play sounds
            _audioSource.PlayOneShot(_clip, 1.0f);
            Destroy(collision.gameObject);
        }
        //if it hits a yellow block,add score and destroy
        else if (collision.gameObject.tag == "yellow")
        {
            //calculate chaining score
            if (_ifHitPaddle == false)
            {
                GameManager.score += (3 + _greyCount);
            }
            //normal score
            else
            {
                GameManager.score += (2 + _greyCount);
            }
            _ifHitPaddle = false;
            //use particle system
            Instantiate(_particle, collision.gameObject.transform.position, Quaternion.identity);
            //play sounds
            _audioSource.PlayOneShot(_clip, 1.0f);
            Destroy(collision.gameObject);
        }
        //if it hits a red block,add score, speed up and destory
      
        else if (collision.gameObject.tag == "red"&&!_redBuffActivated)
        {
            //calculate chaining score
            if (_ifHitPaddle == false)
            {
                GameManager.score += 6;
            }
            //normal score
            else
            {
                GameManager.score += 5;
            }
            _redBuffActivated = true;     
            //speed up the ball
            StartCoroutine(SpeedUp());
            _ifHitPaddle = false;
            //use particle system
            Instantiate(_particle, collision.gameObject.transform.position, Quaternion.identity);
            //play sounds
            _audioSource.PlayOneShot(_clip, 1.0f);
            Destroy(collision.gameObject);
        }
        //if hit another red block within the buff time, we extend the buff time
        else if (collision.gameObject.tag == "red" && _redBuffActivated)
        {
            speedUpTime += 5;
            //use particle system
            Instantiate(_particle, collision.gameObject.transform.position, Quaternion.identity);
            //play sounds
            _audioSource.PlayOneShot(_clip, 1.0f);
            Destroy(collision.gameObject);
        }

        else 
        {
            //play sounds
            _audioSource.PlayOneShot(_clip,1.0f);
            Destroy(collision.gameObject);
        }

    


        //if gameover
        if (isGameOver)
        {
            //set velocity to 0
            velocity = 0;
            //disable rigidbody2d
            _rb.Sleep();
            //stop the coroutine
            StopCoroutine(SpeedUp());
            //call gameover function and reload the scene
            _gameManager.gameOver();

        }
        else
        {

            //reflect the direction of the ball along with the normal of contacting point
            _direction = Vector3.Reflect(_direction, collision.contacts[0].normal);
            _direction.Normalize();
        }
      
    }

    //speed up the ball for 5 seconds
    IEnumerator SpeedUp()
    {
        //record the original speed and color
        float originalVelocity = velocity;
        Color originalColor = _spriterenderer.color;
        while (speedUpTime >=0)
        {
            //double the speed
            velocity =originalVelocity* 2;
            speedUpTime -= Time.smoothDeltaTime;
            //change the balls color when the buff the activated
            _spriterenderer.color = new Color(0.8313f,0.2425f,0.2235f,1.0f);
            //if gameover stop the ball
            if (isGameOver)
                velocity = 0;
            yield return null;

        }
        //reset the speeduptime, speed and color
        speedUpTime = 5.0f;
        velocity = originalVelocity;
        _spriterenderer.color = originalColor;
        //set the buff state to false
        _redBuffActivated = false;
        
    }

 
}
