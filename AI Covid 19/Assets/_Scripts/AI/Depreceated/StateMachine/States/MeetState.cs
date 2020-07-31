namespace Covid19.AI.Depreceated.StateMachine.States
{
    internal class MeetState : FSMState
    {
        public MeetState(FiniteStateMachine finiteStateMachine) : base(finiteStateMachine)
        {
        }

        //private List<Tuple<Bot, float>> _listIgnoredBots = new List<Tuple<Bot, float>>();
        //bool CanSeeObject(Transform initial, Transform target, float dist, float angle)
        //{
        //    Vector3 diferenta = target.position - initial.position;
        //    RaycastHit hit;
        //    if (Vector3.Distance(initial.position, target.position) <= dist)
        //    {
        //        if (Vector3.Angle(initial.forward, diferenta) <= angle / 2)
        //        {
        //            if (Physics.Raycast(initial.position, diferenta, out hit))
        //            {
        //                if (hit.collider.gameObject.GetComponent<Bot>() != null)
        //                {
        //                    return true;
        //                }
        //            }
        //            else
        //                return true;
        //        }
        //    }
        //    return false;
        //}
        //void PrepareForMeeting(Bot bot, Bot other, Vector3 currentMeetingPosition, int meetingDuration)
        //{
        //    bot._lastState = bot._currentState;
        //    bot._currentState = State.Meet;
        //    // pregatesc pentru intalnire botul bot si botul other pentru intalnire
        //    bot._agent.isStopped = false; // ma asigur ca nu sunt opriti
        //    bot._agent.destination = currentMeetingPosition; // pun destinatia
        //    bot.meetingPoint = currentMeetingPosition; // spun meeting point de vazut in inspector, nu fac nimic din cod cu el
        //    bot._inMeeting = true; // sunt in drum spre intalnire
        //    bot._meetingBot = other;// cu cine se intalneste
        //    bot._talkDuration = meetingDuration; // cat timp dureaza intalnirea
        //}

        //private int CompareBot(Bot a, Bot b)
        //{
        //    // pt a compara botii a si b in functie de distanta. returneaza bot-ul cel mai apropiat de cel curent
        //    float dista = Vector3.Distance(transform.position, a.gameObject.transform.position);

        //    float distb = Vector3.Distance(transform.position, b.gameObject.transform.position);
        //    if (dista == distb)
        //        return 0;
        //    if (dista < distb)
        //        return -1; // botul a
        //    else
        //        return 1; // botul b
        //}
        //void TryMeetBot()
        //{

        //    List<Bot> possibleBots = new List<Bot>(); /// lista cu toti botii posibili
        //    foreach (Bot partnerBot in Bot.listBots)
        //    {
        //        if (_listIgnoredBots.Find(bot => bot.Item1 == partnerBot) != null)// daca botul partner bot este ignorat
        //        {
        //            float startIgnoringTime = _listIgnoredBots.Find(bot => bot.Item1 == partnerBot).Item2;
        //            /// daca sa zicem bot1 si bot2 incearca sa se intalneasca dar nu reusesc din cauza probabilitatii
        //            ///  atunci se ignora timp de 10 pana cand pot sa mai incerce inca o data sa se intalneasca
        //            ///  10 secunde e timp standard,
        //            if (Time.time - startIgnoringTime < 10f)
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                _listIgnoredBots.Remove(Tuple.Create(partnerBot, startIgnoringTime));
        //            }
        //        }
        //        /// daca botul pe care il caut este diferit de botul curent si daca botul curent nu este in meeting si 
        //        /// daca partnetBot la fel nu este in meeting il adaug la lista de boti posibili
        //        if (partnerBot != currentBot && !currentBot. && !partnerBot._inMeeting
        //            && CanSeeObject(_head.transform, partnerBot._head.transform, _viewDistance, _viewAngle)
        //            && CanSeeObject(partnerBot._head.transform, _head.transform, partnerBot._viewDistance, partnerBot._viewAngle))
        //        {
        //            Debug.Log("<color=red>Can see each other</color>");
        //            possibleBots.Add(partnerBot);
        //        }
        //    }
        //    if (possibleBots.Count == 0)
        //        return; /// nu a vazut pe nimeni care sa fie disponibil
        //    Debug.Log("sunt : " + possibleBots.Count);
        //    possibleBots.Sort(CompareBot); /// sortez botii dupa distanta

        //    foreach (Bot partnerBot in possibleBots)
        //    {
        //        /// daca nu sunt meeting nici botul curent nici partnerbot si daca celalat bot nu l-a ignorat pe cel curent
        //        /// adica daca partnerbot.lista nu are un tuple care sa aiba botul curent ca si item1
        //        if (!_inMeeting && !partnerBot._inMeeting && partnerBot._listIgnoredBots.Find(bot => this == bot.Item1) == null &&
        //            _currentState != State.AnyAction && partnerBot._currentState != State.AnyAction)
        //        {
        //            int randomValueBetweenRange = UnityEngine.Random.Range(1, 10); // trebuie verificat sa fie la fel cu Range[1,10] sau daca e modificat in viitor
        //                                                                           // daca randomul este mai mic decat nivelul social al primului si este mai mic decat nivelul social al ceiluilat
        //                                                                           // atunci se pot intalni
        //            if (randomValueBetweenRange <= _sociableLevel && randomValueBetweenRange <= partnerBot._sociableLevel)
        //            {
        //                //Debug.Log("se intalnesc cei doi boti: " + this.name + "si" + partnerBot.name);

        //                Vector3 diferenta = partnerBot.transform.position - transform.position;/// vectorul de la botul curent la celelalt
        //                diferenta /= 2; // jumatatea vectorului
        //                Vector3 meetingPointHalf = transform.position + diferenta; // punctul de intalnire la jumatate
        //                Vector3 offset = diferenta.normalized * _offsetMeeting / 2; // offset pt inalnire

        //                int average = (_sociableLevel + partnerBot._sociableLevel) / 2;
        //                int sum = _sociableLevel + partnerBot._sociableLevel;
        //                int randomTime = UnityEngine.Random.Range(average, sum);  // aleg un numar random de secunde pentru timpul de intalnire
        //                                                                          // this-> bot curent, partnetBot-> botul cu care se intalneste
        //                PrepareForMeeting(this, partnerBot, meetingPointHalf - offset, randomTime); // pregatesc botul curent pentru intalnire
        //                PrepareForMeeting(partnerBot, this, meetingPointHalf + offset, randomTime); // pregatesc celelalt bot pentru intalnire
        //                break; // o data ce am gasit un partener de intalnire atunci inchid pentru ca numai are sens sa caut altul.
        //            }
        //            else
        //            {

        //                _listIgnoredBots.Add(Tuple.Create(partnerBot, Time.time));
        //                // ignor botul pentru un anumit timp, retin timpul la care a inceput sa fie ignorat

        //            }
        //        }
        //    }

        //}
    }
}