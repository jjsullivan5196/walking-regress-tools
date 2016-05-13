setwd("C:/Users/gameprogrammer/Documents/walking-regress-tools/Assets/Resources")
source("funcs.R")

#smoothconstant = 0.002

byTime = FALSE

acc = read.csv("daniel-walking-acc.csv", header = FALSE, col.names = c("comx","comy","comz","time"))
gyro = read.csv("daniel-walking-gyro.csv", header = FALSE, col.names = c("comx","comy","comz","time"))

#acc_smooth = data.frame(comx = predict(loess(comx ~ time, data = acc, span = smoothconstant)), comy = predict(loess(comy ~ time, data = acc, span = smoothconstant)), comz = predict(loess(comz ~ time, data = acc, span = smoothconstant)), time = acc$time)

#dan
acc = zero_time(acc, 248, 286)
gyro = zero_time(gyro, 248, 286)

#john
#acc = zero_time(acc, 102, 138)
#gyro = zero_time(gyro, 102, 138)

#acc = zero_time(acc, 600, 650)
#gyro = zero_time(gyro, 600, 650)

acc$comy = acc$comy * acc$comz
gyro$comy = acc$comy * acc$comz


par(mfcol = c(3, 2))

if(!byTime) {
  plot(acc$comx, type = "l", col = "red")
  plot(acc$comy, type = "l", col = "green")
  plot(acc$comz, type = "l", col = "blue")
  
  plot(gyro$comx, type = "l", col = "red")
  plot(gyro$comy, type = "l", col = "green")
  plot(gyro$comz, type = "l", col = "blue")
}
if(byTime) {
  plot(acc$time, acc$comx, type = "p", col = "red")
  plot(acc$time, acc$comy, type = "p", col = "green")
  plot(acc$time, acc$comz, type = "p", col = "blue")
  
  plot(gyro$time, gyro$comx, type = "p", col = "red")
  plot(gyro$time, gyro$comy, type = "p", col = "green")
  plot(gyro$time, gyro$comz, type = "p", col = "blue")
}

#dan
#write.table(acc ,"daniel-single-step2-acc.csv", sep = ',', col.names = FALSE, row.names = FALSE)
#write.table(gyro ,"daniel-single-step2-gyro.csv", sep = ',', col.names = FALSE, row.names = FALSE)

#john
#write.table(acc ,"john-single-step2-acc.csv", sep = ',', col.names = FALSE, row.names = FALSE)
#write.table(gyro ,"john-single-step2-gyro.csv", sep = ',', col.names = FALSE, row.names = FALSE)

#stupidity
#write.table(acc_step_rise ,"acc-step-rise.csv", sep = ',', col.names = FALSE, row.names = FALSE)
#write.table(acc_step_cont ,"acc-step-cont.csv", sep = ',', col.names = FALSE, row.names = FALSE)
#write.table(acc_step_fall ,"acc-step-fall.csv", sep = ',', col.names = FALSE, row.names = FALSE)

#sensor fusion ha
write.table(acc ,"daniel-single-step-fused-acc.csv", sep = ',', col.names = FALSE, row.names = FALSE)
write.table(gyro ,"daniel-single-step-fused-gyro.csv", sep = ',', col.names = FALSE, row.names = FALSE)