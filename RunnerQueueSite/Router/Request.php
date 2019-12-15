<?php
include_once 'IRequest.php';

class Request implements IRequest
{
  function __construct()
  {
    $this->bootstrapSelf();
  }

  private function bootstrapSelf()
  {
    foreach($_SERVER as $key => $value)
    {
      $this->{$this->toCamelCase($key)} = $value;
    }
  }

  private function toCamelCase($string)
  {
    $result = strtolower($string);
        
    preg_match_all('/_[a-z]/', $result, $matches);

    foreach($matches[0] as $match)
    {
        $c = str_replace('_', '', strtoupper($match));
        $result = str_replace($match, $c, $result);
    }

    return $result;
  }

  public function getFormBody()
  {
    if($this->requestMethod === "GET")
    {
      return;
    }


    if ($this->requestMethod == "POST")
    {
        if(isset($_POST)):
            print('Request: post!');
            var_dump($_POST);
            foreach($_POST as $key => $val)
            {
                if(filter_var($val, FILTER_VALIDATE_EMAIL)):
                    $data[$key] = filter_input(INPUT_POST, $key, FILTER_SANITIZE_EMAIL);
                else:
                    $data[$key] = filter_input(INPUT_POST, $key, FILTER_SANITIZE_SPECIAL_CHARS);
                endif;
            }
            return $data;
        endif;
        
        return false;
    }
  }
  
  public function getBody()
  {
      if ($this->requestMethod === "GET")
      {
          return;
      }
      
      if ($this->requestMethod === "POST" || $this->requestMethod === "PUT")
      {
        // Get update parameters
        $body = file_get_contents('php://input');
        
        switch(strtolower($_SERVER["CONTENT_TYPE"]))
        {
            case "application/json":
                $data = json_decode($body);
                break;
            case "text/xml":
                return false;
                break;
            default:
                return false;
                break;
        }
        return $data;
      }
  }
}