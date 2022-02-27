const express = require("express");
const bodyParser = require("body-parser");
const app = express();

app.use(bodyParser.urlencoded());
app.use(bodyParser.json());

var data = new Date();
var setHour = 00;
var respawned = false;



let posizioniSfere = {
    entries: [
        {
            dead: false,
            index: 0,
            x: 0,
            y: 0
        },
        {
            dead: false,
            index: 1,
            x: 1,
            y: 1
        }
    ]
}

app.listen(3000, () => {
    console.log("Server operativo")
    ResettaSfere();
    setInterval(ResettaSfere, 100*60);
});

app.post("/set_dead", (req, res, ind) => {
    console.log(req.body.index);
    posizioniSfere.entries[req.body.index].dead = true;
});

app.get("/GiocoSfere", (req, res) => {
    res.send(JSON.stringify(posizioniSfere));
});

function ResettaSfere()
{
    var hour = data.getHours();

    if(hour != setHour)
    {
        respawned = false;
    }

    if(!respawned && hour == setHour)
    {
        posizioniSfere.entries.forEach((e, ind) =>{
            console.log("Dead check: " + e.dead);
            if(e.dead == true)
            {
                posizioniSfere.entries[ind].dead = false;
                console.log("Index: " + posizioniSfere.entries[ind].index + 
                " Is Dead: " + posizioniSfere.entries[ind].dead);
            }
        });

        respawned = true;
    }
}