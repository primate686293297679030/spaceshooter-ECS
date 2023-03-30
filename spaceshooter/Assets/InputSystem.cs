using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class InputSystem : SystemBase
{
    public bool shooting;
    int currentX=-1;
    int currentY=-1;
    bool[] Activeinputs = new bool[4];
    public Vector3 projectileDir = new Vector3(0,0,0);
    protected override void OnCreate()
    {
        Enabled = false;
    }
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().ForEach((ref Translation translation, ref PlayerComponent playerComponent) =>
        {
           
            translation.Value.x += Input.GetAxisRaw("Horizontal") * Time.DeltaTime*3;
            translation.Value.y += Input.GetAxisRaw("Vertical") * Time.DeltaTime*3;
            translation.Value.z = 0;
            playerComponent.position = translation.Value;

        }).Run();
   
        // Allows player to hold down many keys at a time and makes shooting less choppy
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            if (Activeinputs[0] == false)
            {
                currentX = 0;
            }

            Activeinputs[0] = true;
        
           
                
            shooting = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          
            if (Activeinputs[1] == false)
            {
                currentX = 1;
            }
            Activeinputs[1] = true;
       
            
         
            shooting = true;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            if(Activeinputs[2]==false)
            {
                currentY = 2;
            }
        
            Activeinputs[2] = true;
            shooting = true;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Activeinputs[3] == false)
            {
                currentY = 3;
            }
          
            Activeinputs[3] = true;
            shooting = true;
        }
        

      
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
          
            

            Activeinputs[0] = false;
            if(Activeinputs[1]==true)
            {

                currentX = 1;
            }
            else
            {
                projectileDir.x = 0;
                currentX = -1;
               
            }


        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
          
            Activeinputs[1] = false;

            if(Activeinputs[0]==true)
            {
                currentX = 0;
            }
            else

            {
                projectileDir.x = 0;
                currentX = -1;
         
            }
        
          
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
           


            Activeinputs[2] = false;
            if (Activeinputs[3] == true)
            {

                currentY = 3;
            }
            else
            {
                projectileDir.y = 0;
                currentY = -1;
           
            }
           
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            Activeinputs[3] = false;
            if (Activeinputs[2] == true)
            {

                currentY = 2;
            }
            else
            {
                projectileDir.y = 0;
                currentY = -1;
            
            }
            
        }

        
        if(currentX==-1 && currentY == -1)
        {
            shooting = false;
        }
        if (currentX == 0)
        {
            projectileDir.x = -1;
        }
        if (currentX == 1)
        {
            projectileDir.x = 1;

        }
        if (currentY == 2)
        {
            projectileDir.y = 1;
        }
        if (currentY == 3)
        {
            projectileDir.y = -1;
        }
        if (HasComponent<isDeadTag>(World.GetExistingSystem<GameHandler>().playerEntity))
        {
            Enabled = false;
        }
    }
}

